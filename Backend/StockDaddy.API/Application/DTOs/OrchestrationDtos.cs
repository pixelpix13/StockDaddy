using StockDaddy.Domain.Enums;

namespace StockDaddy.Application.DTOs;

public class CreateProductWithVariantRequest
{
    public int TenantId { get; set; }
    public int StoreId { get; set; }
    public int? SubcategoryId { get; set; }

    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Unit { get; set; } = "pcs";

    public string SkuCode { get; set; } = string.Empty;
    public int HsnCodeId { get; set; } = 1;
    public decimal CostPrice { get; set; }
    public decimal Price { get; set; }
    public decimal TaxPercent { get; set; } = 18;
    public int InitialQuantity { get; set; }
}

public class ProductWithVariantResponse
{
    public ProductDto Product { get; set; } = null!;
    public ProductVariantDto Variant { get; set; } = null!;
}

public class CheckoutLineRequest
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
}

public class CheckoutCustomerRequest
{
    public int? CustomerId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Phone { get; set; } = string.Empty;
    public string Email { get; set; } = string.Empty;
    public string Address { get; set; } = string.Empty;
}

public class CheckoutSaleRequest
{
    public int TenantId { get; set; }
    public int StoreId { get; set; }
    public int SoldBy { get; set; }
    public int? CustomerId { get; set; }
    public CheckoutCustomerRequest? Customer { get; set; }
    public PaymentMethod PaymentMethod { get; set; } = PaymentMethod.Cash;
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercent { get; set; }
    /// <summary>Required when PaymentMethod is Credit — when payment is expected.</summary>
    public DateTime? CreditDueDate { get; set; }
    public string? Notes { get; set; }
    public List<CheckoutLineRequest> Items { get; set; } = new();
}

public class CheckoutLineResponse
{
    public int ProductVariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public int Quantity { get; set; }
    public decimal UnitPrice { get; set; }
    public decimal LineSubtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal LineTotal { get; set; }
}

public class CheckoutSaleResponse
{
    public int SaleId { get; set; }
    public int CustomerId { get; set; }
    public decimal Subtotal { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TotalAmount { get; set; }
    public PaymentMethod PaymentMethod { get; set; }
    public int? CreditLedgerId { get; set; }
    public List<CheckoutLineResponse> Items { get; set; } = new();
}

public class AdjustStockRequest
{
    public int ProductVariantId { get; set; }
    public int QuantityChange { get; set; }
    public string? Reason { get; set; }
}

public class AdjustStockResponse
{
    public int ProductVariantId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public int PreviousQuantity { get; set; }
    public int NewQuantity { get; set; }
}

public class CreatePurchaseOrderLineRequest
{
    public int ProductVariantId { get; set; }
    public int Quantity { get; set; }
    public decimal UnitCost { get; set; }
}

public class CreatePurchaseOrderWithItemsRequest
{
    public int TenantId { get; set; }
    public int SupplierId { get; set; }
    public int StoreId { get; set; }
    public DateTime OrderDate { get; set; }
    public DateTime ExpectedDelivery { get; set; }
    public PurchaseOrderStatus Status { get; set; } = PurchaseOrderStatus.Pending;
    public string Notes { get; set; } = string.Empty;
    public decimal? TotalAmount { get; set; }
    public DateTime? DueDate { get; set; }
    public List<CreatePurchaseOrderLineRequest> Items { get; set; } = new();
}

public class PurchaseOrderWithItemsResponse
{
    public PurchaseOrderDto Order { get; set; } = null!;
    public List<PurchaseItemDto> Items { get; set; } = new();
}

public class VariantStockDto
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public string ProductName { get; set; } = string.Empty;
    public int StoreId { get; set; }
    public string VariantName { get; set; } = string.Empty;
    public string SkuCode { get; set; } = string.Empty;
    public decimal Price { get; set; }
    public decimal CostPrice { get; set; }
    public decimal TaxPercent { get; set; }
    public int HsnCodeId { get; set; }
    public int Quantity { get; set; }
    public int? SubcategoryId { get; set; }
    public string? SubcategoryName { get; set; }
}
