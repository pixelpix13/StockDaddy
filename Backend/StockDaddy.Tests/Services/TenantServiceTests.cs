using StockDaddy.Application.DTOs;
using StockDaddy.Application.Services;
using StockDaddy.Tests.Fakes;

namespace StockDaddy.Tests.Services;

public class TenantServiceTests
{
    [Fact]
    public async Task UpdateAsync_WhenTenantDoesNotExist_ReturnsNull()
    {
        // Arrange
        var repo = new FakeTenantRepository(); // empty — no tenants
        var service = new TenantService(repo);

        // Act
        var result = await service.UpdateAsync(99, new UpdateTenantRequest { Name = "Ghost" });

        // Assert
        Assert.Null(result);
    }
}