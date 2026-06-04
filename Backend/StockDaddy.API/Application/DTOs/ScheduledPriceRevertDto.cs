namespace StockDaddy.Application.DTOs
{
    public class ScheduledPriceRevertDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public int RefId { get; set; }
        public string OriginalPricesJson { get; set; } = string.Empty;
        public DateTime RevertAt { get; set; }
        public bool IsCompleted { get; set; }
        public int CreatedBy { get; set; }
        public DateTime CreatedAt { get; set; }
        public string? BatchCriteria { get; set; }
    }
}
