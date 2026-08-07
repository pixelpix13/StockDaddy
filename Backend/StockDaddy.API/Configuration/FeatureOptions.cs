namespace StockDaddy.API.Configuration;

public class FeatureOptions
{
    public BillAdjustmentFeatureOptions BillAdjustment { get; set; } = new();
}

public class BillAdjustmentFeatureOptions
{
    public bool Enabled { get; set; }
}
