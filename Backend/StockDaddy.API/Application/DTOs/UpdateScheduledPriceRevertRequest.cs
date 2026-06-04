namespace StockDaddy.Application.DTOs
{
    public class UpdateScheduledPriceRevertRequest
    {
        public DateTime? RevertAt { get; set; }
        public bool? IsCompleted { get; set; }
        public string? OriginalPricesJson { get; set; }
        public string? BatchCriteria { get; set; }
    }
}
