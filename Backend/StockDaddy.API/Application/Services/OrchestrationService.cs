using Microsoft.EntityFrameworkCore;
using StockDaddy.Application.DTOs;
using StockDaddy.Domain.Entities;
using StockDaddy.Domain.Enums;
using StockDaddy.Infrastructure.Persistence;

namespace StockDaddy.Application.Services;

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

        await using var transaction = await _context.Database.BeginTransactionAsync();
        try
        {
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

            var totalAmount = subtotal + taxTotal;
            var now = DateTime.UtcNow;

            var sale = new Sale
            {
                TenantId = request.TenantId,
                StoreId = request.StoreId,
                CustomerId = request.CustomerId,
                SoldBy = request.SoldBy,
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

            await _context.SaveChangesAsync();
            await transaction.CommitAsync();

            return new CheckoutSaleResponse
            {
                SaleId = sale.Id,
                Subtotal = subtotal,
                TaxAmount = taxTotal,
                TotalAmount = totalAmount,
                PaymentMethod = request.PaymentMethod,
                Items = lineResponses
            };
        }
        catch
        {
            await transaction.RollbackAsync();
            throw;
        }
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
            var order = new PurchaseOrder
            {
                TenantId = request.TenantId,
                SupplierId = request.SupplierId,
                StoreId = request.StoreId,
                OrderDate = request.OrderDate,
                ExpectedDelivery = request.ExpectedDelivery,
                Status = request.Status,
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

    public async Task<List<VariantStockDto>> GetVariantStockAsync(int? storeId = null)
    {
        var query = _context.ProductVariants
            .AsNoTracking()
            .Include(v => v.Product)
                .ThenInclude(p => p!.Subcategory)
            .Where(v => !v.IsDeleted && v.Product != null && !v.Product.IsDeleted);

        if (storeId.HasValue)
        {
            query = query.Where(v => v.StoreId == storeId.Value);
        }

        return await query
            .OrderBy(v => v.Product!.Name)
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
            .ToListAsync();
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
        Notes = order.Notes,
        CreatedAt = order.CreatedAt,
        UpdatedAt = order.UpdatedAt
    };
}
