using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Helpers;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Application.Services;

/// <summary>
/// Cross-entity workflows that must run in a single DB transaction:
/// product+variant+stock, POS checkout, stock adjust, PO with lines, barcode lookup.
/// Controllers should stay thin; business rules live here.
/// </summary>
public class OrchestrationService
{
    private readonly ApplicationDbContext _context;

    public OrchestrationService(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<ProductWithVariantResponse?> CreateProductWithVariantAsync(CreateProductWithVariantRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.Name) || string.IsNullOrWhiteSpace(request.SkuCode))
        {
            return null;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;
            var marginPercent = request.CostPrice > 0
                ? ((request.Price - request.CostPrice) / request.CostPrice) * 100
                : 0;

            var product = new Product
            {
                TenantId = request.TenantId,
                StoreId = request.StoreId,
                SubcategoryId = request.SubcategoryId,
                Name = request.Name.Trim(),
                Description = request.Description ?? string.Empty,
                Unit = request.Unit,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _context.Products.AddAsync(product);
            await _context.SaveChangesAsync();

            var variant = new ProductVariant
            {
                ProductId = product.Id,
                StoreId = request.StoreId,
                HSNCodeId = request.HsnCodeId,
                VariantName = request.Name.Trim(),
                Barcode = request.SkuCode.Trim(),
                SkuCode = request.SkuCode.Trim(),
                CostPrice = request.CostPrice,
                MarginPercent = marginPercent,
                TaxPercent = request.TaxPercent,
                Price = request.Price,
                Quantity = request.InitialQuantity,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _context.ProductVariants.AddAsync(variant);
            await _context.SaveChangesAsync();

            if (request.InitialQuantity > 0)
            {
                await SyncStockItemAsync(product.Id, request.StoreId, request.InitialQuantity, now);
            }

            await transaction.CommitAsync();

            return new ProductWithVariantResponse
            {
                Product = MapProduct(product),
                Variant = MapVariant(variant)
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<CheckoutSaleResponse?> CheckoutSaleAsync(CheckoutSaleRequest request)
    {
        if (request.Items.Count == 0)
        {
            return null;
        }

        var customerInput = request.Customer;
        if (customerInput == null || string.IsNullOrWhiteSpace(customerInput.Name) || string.IsNullOrWhiteSpace(customerInput.Phone))
        {
            throw new InvalidOperationException("Customer name and phone are required for checkout.");
        }

        if (request.PaymentMethod == PaymentMethod.Credit && !request.CreditDueDate.HasValue)
        {
            throw new InvalidOperationException("Credit due date is required for credit sales.");
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var customerId = await ResolveCustomerForCheckoutAsync(request.TenantId, customerInput);

            var variantIds = request.Items.Select(i => i.ProductVariantId).Distinct().ToList();
            var variants = await _context.ProductVariants
                .Include(v => v.Product)
                .Where(v => variantIds.Contains(v.Id) && !v.IsDeleted)
                .ToDictionaryAsync(v => v.Id);

            if (variants.Count != variantIds.Count)
            {
                return null;
            }

            foreach (var line in request.Items)
            {
                var variant = variants[line.ProductVariantId];
                if (variant.Quantity < line.Quantity)
                {
                    throw new InvalidOperationException(
                        $"Insufficient stock for {variant.SkuCode}. Available: {variant.Quantity}, requested: {line.Quantity}.");
                }
            }

            var lineResponses = new List<CheckoutLineResponse>();
            decimal subtotal = 0;
            decimal taxTotal = 0;

            foreach (var line in request.Items)
            {
                var variant = variants[line.ProductVariantId];
                var unitPrice = variant.Price;
                var lineSubtotal = unitPrice * line.Quantity;
                var taxAmount = Math.Round(lineSubtotal * (variant.TaxPercent / 100m), 2);
                var lineTotal = lineSubtotal + taxAmount;

                subtotal += lineSubtotal;
                taxTotal += taxAmount;

                lineResponses.Add(new CheckoutLineResponse
                {
                    ProductVariantId = variant.Id,
                    VariantName = variant.VariantName,
                    SkuCode = variant.SkuCode,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    LineSubtotal = lineSubtotal,
                    TaxAmount = taxAmount,
                    LineTotal = lineTotal
                });
            }

            var grossTotal = subtotal + taxTotal;
            var discount = request.DiscountAmount;
            if (request.DiscountPercent > 0)
            {
                discount = Math.Max(discount, Math.Round(grossTotal * request.DiscountPercent / 100m, 2));
            }

            discount = Math.Min(Math.Max(discount, 0), grossTotal);
            var totalAmount = grossTotal - discount;
            var now = DateTime.UtcNow;

            var sale = new Sale
            {
                TenantId = request.TenantId,
                StoreId = request.StoreId,
                CustomerId = customerId,
                SoldBy = request.SoldBy,
                SubtotalAmount = subtotal,
                TaxAmount = taxTotal,
                DiscountAmount = discount,
                TotalAmount = totalAmount,
                PaymentMethod = request.PaymentMethod,
                Notes = request.Notes,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _context.Sales.AddAsync(sale);
            await _context.SaveChangesAsync();

            foreach (var line in request.Items)
            {
                var variant = variants[line.ProductVariantId];
                var unitPrice = variant.Price;

                await _context.SaleItems.AddAsync(new SaleItem
                {
                    SaleId = sale.Id,
                    ProductVariantId = variant.Id,
                    Quantity = line.Quantity,
                    UnitPrice = unitPrice,
                    TotalPrice = unitPrice * line.Quantity,
                    IsDeleted = false
                });

                variant.Quantity -= line.Quantity;
                variant.UpdatedAt = now;

                if (variant.ProductId > 0)
                {
                    await SyncStockItemAsync(variant.ProductId, variant.StoreId, -line.Quantity, now);
                }
            }

            int? creditLedgerId = null;
            if (request.PaymentMethod == PaymentMethod.Credit)
            {
                var customer = await _context.Customers.FirstAsync(c => c.Id == customerId);
                var ledger = new CreditLedger
                {
                    TenantId = request.TenantId,
                    PartyType = CreditPartyType.Customer,
                    Status = CreditStatus.Pending,
                    CustomerId = customerId,
                    SaleId = sale.Id,
                    PartyName = customer.Name,
                    PartyPhone = customer.Phone,
                    PartyEmail = customer.Email,
                    PartyAddress = customer.Address,
                    Amount = totalAmount,
                    AmountPaid = 0,
                    DueDate = request.CreditDueDate!.Value.ToUniversalTime(),
                    Notes = $"Sale #{sale.Id} on credit",
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                };
                await _context.CreditLedgers.AddAsync(ledger);
                await _context.SaveChangesAsync();
                creditLedgerId = ledger.Id;
            }

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CheckoutSaleResponse
            {
                SaleId = sale.Id,
                CustomerId = customerId,
                Subtotal = subtotal,
                TaxAmount = taxTotal,
                DiscountAmount = discount,
                TotalAmount = totalAmount,
                PaymentMethod = request.PaymentMethod,
                CreditLedgerId = creditLedgerId,
                Items = lineResponses
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    private async Task<int> ResolveCustomerForCheckoutAsync(int tenantId, CheckoutCustomerRequest customer)
    {
        var now = DateTime.UtcNow;
        Customer? entity = null;

        if (customer.CustomerId.HasValue)
        {
            entity = await _context.Customers
                .FirstOrDefaultAsync(c => c.Id == customer.CustomerId.Value && c.TenantId == tenantId && !c.IsDeleted);
        }

        if (entity == null && !string.IsNullOrWhiteSpace(customer.Phone))
        {
            var phone = customer.Phone.Trim();
            entity = await _context.Customers
                .FirstOrDefaultAsync(c => c.TenantId == tenantId && !c.IsDeleted && c.Phone == phone);
        }

        if (entity == null)
        {
            entity = new Customer
            {
                TenantId = tenantId,
                Name = customer.Name.Trim(),
                Phone = customer.Phone.Trim(),
                Email = customer.Email?.Trim() ?? string.Empty,
                Address = customer.Address?.Trim() ?? string.Empty,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };
            await _context.Customers.AddAsync(entity);
            await _context.SaveChangesAsync();
            return entity.Id;
        }

        entity.Name = customer.Name.Trim();
        entity.Phone = customer.Phone.Trim();
        entity.Email = customer.Email?.Trim() ?? entity.Email;
        entity.Address = customer.Address?.Trim() ?? entity.Address;
        entity.UpdatedAt = now;
        await _context.SaveChangesAsync();
        return entity.Id;
    }

    public async Task<AdjustStockResponse?> AdjustStockAsync(AdjustStockRequest request)
    {
        var variant = await _context.ProductVariants
            .FirstOrDefaultAsync(v => v.Id == request.ProductVariantId && !v.IsDeleted);

        if (variant == null)
        {
            return null;
        }

        var previous = variant.Quantity;
        var updated = previous + request.QuantityChange;
        if (updated < 0)
        {
            throw new InvalidOperationException("Stock quantity cannot go below zero.");
        }

        var now = DateTime.UtcNow;
        variant.Quantity = updated;
        variant.UpdatedAt = now;

        await SyncStockItemAsync(variant.ProductId, variant.StoreId, request.QuantityChange, now);
        await _context.SaveChangesAsync();

        return new AdjustStockResponse
        {
            ProductVariantId = variant.Id,
            VariantName = variant.VariantName,
            SkuCode = variant.SkuCode,
            PreviousQuantity = previous,
            NewQuantity = updated
        };
    }

    public async Task<PurchaseOrderWithItemsResponse?> CreatePurchaseOrderWithItemsAsync(
        CreatePurchaseOrderWithItemsRequest request)
    {
        if (request.Items.Count == 0)
        {
            return null;
        }

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;
            var totalAmount = request.Items.Sum(line => line.UnitCost * line.Quantity);
            var order = new PurchaseOrder
            {
                TenantId = request.TenantId,
                SupplierId = request.SupplierId,
                StoreId = request.StoreId,
                OrderDate = request.OrderDate,
                ExpectedDelivery = request.ExpectedDelivery,
                Status = request.Status,
                TotalAmount = totalAmount,
                DueDate = request.DueDate,
                Notes = request.Notes,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            };

            await _context.PurchaseOrders.AddAsync(order);
            await _context.SaveChangesAsync();

            var itemDtos = new List<PurchaseItemDto>();
            foreach (var line in request.Items)
            {
                var entity = new PurchaseItem
                {
                    PurchaseOrderId = order.Id,
                    ProductVariantId = line.ProductVariantId,
                    Quantity = line.Quantity,
                    UnitCost = line.UnitCost,
                    TotalCost = line.UnitCost * line.Quantity,
                    CreatedAt = now,
                    UpdatedAt = now,
                    IsDeleted = false
                };

                await _context.PurchaseItems.AddAsync(entity);
            }

            await _context.SaveChangesAsync();

            if (request.Status == PurchaseOrderStatus.Unpaid && request.DueDate.HasValue)
            {
                var supplier = await _context.Suppliers
                    .FirstOrDefaultAsync(s => s.Id == request.SupplierId && !s.IsDeleted);
                if (supplier != null)
                {
                    await _context.CreditLedgers.AddAsync(new CreditLedger
                    {
                        TenantId = request.TenantId,
                        PartyType = CreditPartyType.Supplier,
                        Status = CreditStatus.Pending,
                        SupplierId = supplier.Id,
                        PurchaseOrderId = order.Id,
                        PartyName = supplier.Name,
                        PartyPhone = supplier.Phone,
                        PartyEmail = supplier.Email,
                        PartyAddress = supplier.Address,
                        Amount = totalAmount,
                        AmountPaid = 0,
                        DueDate = request.DueDate.Value.ToUniversalTime(),
                        Notes = $"Purchase order #{order.Id} on credit",
                        CreatedAt = now,
                        UpdatedAt = now,
                        IsDeleted = false
                    });
                    await _context.SaveChangesAsync();
                }
            }

            itemDtos = await _context.PurchaseItems
                .Where(i => i.PurchaseOrderId == order.Id && !i.IsDeleted)
                .Select(i => new PurchaseItemDto
                {
                    Id = i.Id,
                    PurchaseOrderId = i.PurchaseOrderId,
                    ProductVariantId = i.ProductVariantId,
                    Quantity = i.Quantity,
                    UnitCost = i.UnitCost,
                    TotalCost = i.TotalCost,
                    CreatedAt = i.CreatedAt,
                    UpdatedAt = i.UpdatedAt
                })
                .ToListAsync();

            await transaction.CommitAsync();

            return new PurchaseOrderWithItemsResponse
            {
                Order = new PurchaseOrderDto
                {
                    Id = order.Id,
                    TenantId = order.TenantId,
                    SupplierId = order.SupplierId,
                    StoreId = order.StoreId,
                    OrderDate = order.OrderDate,
                    ExpectedDelivery = order.ExpectedDelivery,
                    Status = order.Status,
                    TotalAmount = order.TotalAmount,
                    DueDate = order.DueDate,
                    Notes = order.Notes,
                    CreatedAt = order.CreatedAt,
                    UpdatedAt = order.UpdatedAt
                },
                Items = itemDtos
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PurchaseOrderDto?> ReceivePurchaseOrderAsync(int purchaseOrderId)
    {
        var order = await _context.PurchaseOrders
            .FirstOrDefaultAsync(o => o.Id == purchaseOrderId && !o.IsDeleted);

        if (order == null)
        {
            return null;
        }

        if (order.Status == PurchaseOrderStatus.Delivered)
        {
            return MapPurchaseOrder(order);
        }

        var items = await _context.PurchaseItems
            .Where(i => i.PurchaseOrderId == purchaseOrderId && !i.IsDeleted)
            .ToListAsync();

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
            var now = DateTime.UtcNow;
            foreach (var item in items)
            {
                var variant = await _context.ProductVariants
                    .FirstOrDefaultAsync(v => v.Id == item.ProductVariantId && !v.IsDeleted);

                if (variant == null)
                {
                    continue;
                }

                variant.Quantity += item.Quantity;
                variant.UpdatedAt = now;
                await SyncStockItemAsync(variant.ProductId, variant.StoreId, item.Quantity, now);
            }

            order.Status = PurchaseOrderStatus.Delivered;
            order.UpdatedAt = now;
            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return MapPurchaseOrder(order);
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
    }

    public async Task<PagedResult<VariantStockDto>> GetVariantStockAsync(int? storeId = null, PagedQuery? query = null)
    {
        var q = RepositoryPaging.Normalize(query ?? new PagedQuery());
        var baseQuery = _context.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p!.Subcategory)
            .Where(v => !v.IsDeleted && v.Product != null && !v.Product.IsDeleted);

        if (storeId.HasValue)
        {
            baseQuery = baseQuery.Where(v => v.StoreId == storeId.Value);
        }

        if (!string.IsNullOrEmpty(q.Search))
        {
            var pattern = RepositoryPaging.LikePattern(q.Search);
            var idMatch = RepositoryPaging.TryParseSearchId(q.Search, out var searchId);
            baseQuery = baseQuery.Where(v =>
                (idMatch && v.Id == searchId) ||
                (idMatch && v.ProductId == searchId) ||
                EF.Functions.ILike(v.Product!.Name, pattern) ||
                EF.Functions.ILike(v.SkuCode, pattern) ||
                EF.Functions.ILike(v.VariantName, pattern) ||
                (v.Product!.Subcategory != null && EF.Functions.ILike(v.Product.Subcategory.Name, pattern)));
        }

        if (!string.IsNullOrEmpty(q.StockFilter))
        {
            baseQuery = q.StockFilter switch
            {
                "out" => baseQuery.Where(v => v.Quantity == 0),
                "low" => baseQuery.Where(v => v.Quantity > 0 && v.Quantity <= 5),
                "in" => baseQuery.Where(v => v.Quantity > 5),
                _ => baseQuery
            };
        }

        if (q.SubcategoryId.HasValue)
        {
            baseQuery = baseQuery.Where(v => v.Product!.SubcategoryId == q.SubcategoryId.Value);
        }

        baseQuery = (q.SortBy?.ToLowerInvariant(), RepositoryPaging.IsDescending(q)) switch
        {
            ("productname", true) => baseQuery.OrderByDescending(v => v.Product!.Name),
            ("productname", false) => baseQuery.OrderBy(v => v.Product!.Name),
            ("skucode", true) => baseQuery.OrderByDescending(v => v.SkuCode),
            ("skucode", false) => baseQuery.OrderBy(v => v.SkuCode),
            ("price", true) => baseQuery.OrderByDescending(v => v.Price),
            ("price", false) => baseQuery.OrderBy(v => v.Price),
            ("quantity", true) => baseQuery.OrderByDescending(v => v.Quantity),
            ("quantity", false) => baseQuery.OrderBy(v => v.Quantity),
            (_, true) => baseQuery.OrderByDescending(v => v.Id),
            _ => baseQuery.OrderBy(v => v.Id),
        };

        var projected = baseQuery.Select(v => new VariantStockDto
        {
            Id = v.Id,
            ProductId = v.ProductId,
            ProductName = v.Product!.Name,
            StoreId = v.StoreId,
            VariantName = v.VariantName,
            SkuCode = v.SkuCode,
            Price = v.Price,
            CostPrice = v.CostPrice,
            TaxPercent = v.TaxPercent,
            HsnCodeId = v.HSNCodeId,
            Quantity = v.Quantity,
            SubcategoryId = v.Product.SubcategoryId,
            SubcategoryName = v.Product.Subcategory != null ? v.Product.Subcategory.Name : null
        });

        return await RepositoryPaging.ExecuteAsync(projected, q);
    }

    public async Task<VariantStockDto?> GetVariantByBarcodeAsync(string code, int storeId)
    {
        if (string.IsNullOrWhiteSpace(code))
        {
            return null;
        }

        var normalized = code.Trim();
        return await _context.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p!.Subcategory)
            .Where(v =>
                !v.IsDeleted &&
                v.StoreId == storeId &&
                v.Product != null &&
                !v.Product.IsDeleted &&
                (v.SkuCode == normalized || v.Barcode == normalized))
            .Select(v => new VariantStockDto
            {
                Id = v.Id,
                ProductId = v.ProductId,
                ProductName = v.Product!.Name,
                StoreId = v.StoreId,
                VariantName = v.VariantName,
                SkuCode = v.SkuCode,
                Price = v.Price,
                CostPrice = v.CostPrice,
                TaxPercent = v.TaxPercent,
                HsnCodeId = v.HSNCodeId,
                Quantity = v.Quantity,
                SubcategoryId = v.Product.SubcategoryId,
                SubcategoryName = v.Product.Subcategory != null ? v.Product.Subcategory.Name : null
            })
            .FirstOrDefaultAsync();
    }

    private async Task SyncStockItemAsync(int productId, int storeId, int quantityChange, DateTime now)
    {
        var stockItem = await _context.StockItems
            .FirstOrDefaultAsync(s =>
                s.ProductId == productId &&
                s.StoreId == storeId &&
                !s.IsDeleted);

        if (stockItem == null && quantityChange > 0)
        {
            await _context.StockItems.AddAsync(new StockItem
            {
                ProductId = productId,
                StoreId = storeId,
                Quantity = quantityChange,
                Status = quantityChange <= 5 ? StockStatus.Low : StockStatus.InStock,
                LastUpdated = now,
                CreatedAt = now,
                UpdatedAt = now,
                IsDeleted = false
            });
            return;
        }

        if (stockItem == null)
        {
            return;
        }

        stockItem.Quantity += quantityChange;
        if (stockItem.Quantity < 0)
        {
            stockItem.Quantity = 0;
        }

        stockItem.Status = stockItem.Quantity switch
        {
            0 => StockStatus.OutOfStock,
            <= 5 => StockStatus.Low,
            _ => StockStatus.InStock
        };
        stockItem.LastUpdated = now;
        stockItem.UpdatedAt = now;
    }

    private static ProductDto MapProduct(Product product) => new()
    {
        Id = product.Id,
        TenantId = product.TenantId,
        StoreId = product.StoreId,
        SubcategoryId = product.SubcategoryId,
        Name = product.Name,
        Description = product.Description,
        Unit = product.Unit,
        CreatedAt = product.CreatedAt,
        UpdatedAt = product.UpdatedAt,
        IsDeleted = product.IsDeleted,
        DeletedAt = product.DeletedAt
    };

    private static ProductVariantDto MapVariant(ProductVariant variant) => new()
    {
        Id = variant.Id,
        ProductId = variant.ProductId,
        StoreId = variant.StoreId,
        HSNCodeId = variant.HSNCodeId,
        VariantName = variant.VariantName,
        Barcode = variant.Barcode,
        SkuCode = variant.SkuCode,
        CostPrice = variant.CostPrice,
        MarginPercent = variant.MarginPercent,
        TaxPercent = variant.TaxPercent,
        Price = variant.Price,
        Quantity = variant.Quantity,
        CreatedAt = variant.CreatedAt,
        UpdatedAt = variant.UpdatedAt
    };

    private static PurchaseOrderDto MapPurchaseOrder(PurchaseOrder order) => new()
    {
        Id = order.Id,
        TenantId = order.TenantId,
        SupplierId = order.SupplierId,
        StoreId = order.StoreId,
        OrderDate = order.OrderDate,
        ExpectedDelivery = order.ExpectedDelivery,
        Status = order.Status,
        TotalAmount = order.TotalAmount,
        DueDate = order.DueDate,
        Notes = order.Notes,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
