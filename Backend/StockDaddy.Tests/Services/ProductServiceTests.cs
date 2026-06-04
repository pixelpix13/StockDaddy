using NSubstitute;
using StockDaddy.Application.DTOs;
using StockDaddy.Application.Interfaces;
using StockDaddy.Application.Services;

namespace StockDaddy.Tests.Services;

// ─────────────────────────────────────────────────────────────────────────────
// UNIT TESTING LESSON — ProductService
//
// Key concepts covered:
//  1. What is a unit test?
//  2. The AAA pattern (Arrange / Act / Assert)
//  3. Mocking dependencies with NSubstitute
//  4. Testing the happy path
//  5. Testing edge cases (null / not found)
//  6. Testing return values vs. side effects
// ─────────────────────────────────────────────────────────────────────────────

public class ProductServiceTests
{
    // ── shared mock & system-under-test ──────────────────────────────────────
    //
    // IProductRepository is an interface — we never want to hit a real database
    // in a unit test. NSubstitute.Substitute.For<T>() creates a fake object
    // that implements the interface. Every method returns default values unless
    // we tell it otherwise with `.Returns(...)`.

    private readonly IProductRepository _repo;
    private readonly ProductService _sut; // sut = System Under Test

    public ProductServiceTests()
    {
        _repo = Substitute.For<IProductRepository>();
        _sut  = new ProductService(_repo);
    }

    // ── LESSON 1: Happy Path ──────────────────────────────────────────────────
    // The simplest test: does the service return what the repository gives it?
    //
    // AAA breakdown:
    //   Arrange — set up the fake data and tell the mock what to return
    //   Act     — call the real method we are testing
    //   Assert  — verify the result is what we expect

    [Fact]
    public async Task GetAllAsync_ReturnsAllProducts()
    {
        // Arrange
        var fakeProducts = new List<ProductDto>
        {
            new() { Id = 1, Name = "Widget A", TenantId = 1 },
            new() { Id = 2, Name = "Widget B", TenantId = 1 },
        };

        _repo.GetAllAsync().Returns(fakeProducts);

        // Act
        var result = await _sut.GetAllAsync();

        // Assert
        Assert.Equal(2, result.Count);
        Assert.Equal("Widget A", result[0].Name);
    }

    // ── LESSON 2: GetById — found ─────────────────────────────────────────────
    // When the product exists the service should hand it straight back.

    [Fact]
    public async Task GetByIdAsync_WhenProductExists_ReturnsProduct()
    {
        // Arrange
        var fake = new ProductDto { Id = 42, Name = "Found It", TenantId = 1 };
        _repo.GetByIdAsync(42).Returns(fake);

        // Act
        var result = await _sut.GetByIdAsync(42);

        // Assert
        Assert.NotNull(result);
        Assert.Equal(42, result.Id);
        Assert.Equal("Found It", result.Name);
    }

    // ── LESSON 3: GetById — NOT found ────────────────────────────────────────
    // When the repository returns null the service should pass that null back.
    // We test the edge case separately so failures are clear.

    [Fact]
    public async Task GetByIdAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange — .Returns((ProductDto?)null) makes the mock return null
        _repo.GetByIdAsync(999).Returns((ProductDto?)null);

        // Act
        var result = await _sut.GetByIdAsync(999);

        // Assert
        Assert.Null(result);
    }

    // ── LESSON 4: AddAsync — verifying a side effect ─────────────────────────
    // AddAsync doesn't have a meaningful return value to check; what matters is
    // that it called _repo.AddAsync exactly once with the right data.
    // NSubstitute lets us verify this with .Received().

    [Fact]
    public async Task AddAsync_CallsRepositoryAddOnce()
    {
        // Arrange
        var request = new CreateProductRequest { Name = "New Widget", TenantId = 1 };

        // After AddAsync we call GetAllAsync internally, so prime it with one item
        var saved = new ProductDto { Id = 1, Name = "New Widget", TenantId = 1 };
        _repo.GetAllAsync().Returns(new List<ProductDto> { saved });

        // Act
        await _sut.AddAsync(request);

        // Assert — did the repo receive exactly 1 call to AddAsync with our request?
        await _repo.Received(1).AddAsync(request);
    }

    // ── LESSON 5: UpdateAsync — product exists ────────────────────────────────
    // UpdateAsync first checks if the product exists. If it does, it updates
    // and returns the fresh DTO. We verify both the return value AND that
    // the repo was asked to update.

    [Fact]
    public async Task UpdateAsync_WhenProductExists_UpdatesAndReturnsUpdatedDto()
    {
        // Arrange
        var before  = new ProductDto { Id = 5, Name = "Old Name",  TenantId = 1 };
        var after   = new ProductDto { Id = 5, Name = "New Name",  TenantId = 1 };
        var request = new UpdateProductRequest { Name = "New Name" };

        _repo.GetByIdAsync(5).Returns(before, after); // first call → before, second → after
        _repo.UpdateAsync(5, request).Returns(Task.CompletedTask);

        // Act
        var result = await _sut.UpdateAsync(5, request);

        // Assert
        Assert.NotNull(result);
        Assert.Equal("New Name", result!.Name);
        await _repo.Received(1).UpdateAsync(5, request);
    }

    // ── LESSON 6: UpdateAsync — product does NOT exist ───────────────────────
    // When GetById returns null the service should bail out early, return null,
    // and never touch the repo's UpdateAsync.

    [Fact]
    public async Task UpdateAsync_WhenProductDoesNotExist_ReturnsNull()
    {
        // Arrange
        _repo.GetByIdAsync(999).Returns((ProductDto?)null);

        // Act
        var result = await _sut.UpdateAsync(999, new UpdateProductRequest { Name = "X" });

        // Assert
        Assert.Null(result);

        // UpdateAsync should NEVER have been called on the repo
        await _repo.DidNotReceive().UpdateAsync(Arg.Any<int>(), Arg.Any<UpdateProductRequest>());
    }

    // ── LESSON 7: SoftDeleteAsync — product exists ───────────────────────────
    // Returns true and calls SoftDelete on the repo.

    [Fact]
    public async Task SoftDeleteAsync_WhenProductExists_ReturnsTrueAndDeletesIt()
    {
        // Arrange
        _repo.GetByIdAsync(7).Returns(new ProductDto { Id = 7, Name = "Delete Me" });

        // Act
        var result = await _sut.SoftDeleteAsync(7);

        // Assert
        Assert.True(result);
        await _repo.Received(1).SoftDeleteAsync(7);
    }

    // ── LESSON 8: SoftDeleteAsync — product does NOT exist ───────────────────
    // Returns false and never calls the repo's delete.

    [Fact]
    public async Task SoftDeleteAsync_WhenProductDoesNotExist_ReturnsFalse()
    {
        // Arrange
        _repo.GetByIdAsync(999).Returns((ProductDto?)null);

        // Act
        var result = await _sut.SoftDeleteAsync(999);

        // Assert
        Assert.False(result);
        await _repo.DidNotReceive().SoftDeleteAsync(Arg.Any<int>());
    }
}
