namespace StockDaddy.Tests;

public class UnitTest1
{
    [Fact]
    public void Test1()
    {
        int a = 1;
        int b = 2;
        int sum = a + b;
        Assert.Equal(3, sum);
    }
}
