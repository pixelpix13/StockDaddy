namespace StockDaddy.Application.DTOs
{
    public class CreateScheduledPriceRevertRequest
    {
        public string Type { get; set; } = string.Empty;
        public int RefId { get; set; }
        public string OriginalPricesJson { get; set; } = string.Empty;
        public DateTime RevertAt { get; set; }
        public int CreatedBy { get; set; }
        public string? BatchCriteria { get; set; }
    }
}
