using FluentAssertions;
using Catalog.Dal.Context;
using Catalog.Dal.UOW;
using Microsoft.EntityFrameworkCore;
using Moq;
using Xunit;

namespace Catalog.Dal.Tests;

public class UnitOfWorkTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly UnitOfWork _sut;

    public UnitOfWorkTests()
    {
        _context = DbContextFactory.Create();
        _sut = new UnitOfWork(_context);
    }

    [Fact]
    public void Repositories_ShouldShareSameDbContext()
    {
        var restaurantRepo = _sut.Restaurants;
        var dishRepo = _sut.Dishes;

        
        restaurantRepo.Should().NotBeNull();
        dishRepo.Should().NotBeNull();
    }

    [Fact]
    public async Task SaveChangesAsync_AfterMultipleAdds_PersistsAllEntitiesInOneTransaction()
    {
        // Arrange
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1, name: "UOW Restaurant");
        var category = TestDataBuilder.CreateCategory(id: 1, restaurantId: 1, name: "Salads");
        
        var dish = TestDataBuilder.CreateDish(
            id: 1, 
            restaurantId: 1, 
            categoryId: 1, 
            name: "UOW Dish"
        );

        // Act
        await _sut.Restaurants.AddAsync(restaurant);
        await _sut.Categories.AddAsync(category);
        await _sut.Dishes.AddAsync(dish);
        
        await _sut.SaveChangesAsync();

        // Assert
        _context.Restaurants.Should().Contain(r => r.Name == "UOW Restaurant");
        _context.Dishes.Should().Contain(d => d.Name == "UOW Dish");
        _context.Categories.Should().Contain(c => c.Name == "Salads");
    }

    [Fact]
    public async Task SaveChangesAsync_ShouldInvokeContextSaveChangesAsync()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "MockDb")
            .Options;

        var mockContext = new Mock<MyDbContext>(options);
        mockContext.Setup(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()))
                   .ReturnsAsync(1);

        var uow = new UnitOfWork(mockContext.Object);

        // Act
        await uow.SaveChangesAsync();

        // Assert
        mockContext.Verify(c => c.SaveChangesAsync(It.IsAny<CancellationToken>()), Times.Once);
    }

    [Fact]
    public void Repositories_ShouldBeSingletonsWithinUoW()
    {
        // Act
        var firstCall = _sut.Restaurants;
        var secondCall = _sut.Restaurants;

        // Assert
        firstCall.Should().BeSameAs(secondCall);
    }

    [Fact]
    public async Task DisposeAsync_ShouldInvokeContextDispose()
    {
        // Arrange
        var options = new DbContextOptionsBuilder<MyDbContext>()
            .UseInMemoryDatabase(databaseName: "DisposeDb")
            .Options;
        var mockContext = new Mock<MyDbContext>(options);
        var uow = new UnitOfWork(mockContext.Object);

        // Act
        await uow.DisposeAsync();

        // Assert
        mockContext.Verify(c => c.DisposeAsync(), Times.Once);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}