namespace StockDaddy.API.Configuration;

/// <summary>
/// Feature flags from appsettings "Features" section. Optional modules check these at startup.
/// </summary>
public class FeatureOptions
{
    public BillAdjustmentFeatureOptions BillAdjustment { get; set; } = new();
}

public class BillAdjustmentFeatureOptions
{
    public bool Enabled { get; set; }
}
