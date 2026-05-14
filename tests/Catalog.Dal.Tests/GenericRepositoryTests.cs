using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;
using Catalog.Dal.Specifications; // Якщо SpecificationEvaluator там
using Microsoft.EntityFrameworkCore;
using Xunit;
using System.Linq.Expressions;

namespace Catalog.Dal.Tests;

public class GenericRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly GenericRepository<Restaurant> _sut;

    public GenericRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new GenericRepository<Restaurant>(_context);
    }

    [Fact]
    public async Task GetByIdAsync_ExistingId_ReturnsEntity()
    {
        // Arrange 
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1);
        await _sut.AddAsync(restaurant);
        await _context.SaveChangesAsync();

        // Act 
        var result = await _sut.GetByIdAsync(restaurant.Id);

        // Assert 
        result.Should().NotBeNull();
        result!.Id.Should().Be(restaurant.Id);
    }

    [Fact]
    public async Task GetAllAsync_ReturnsAllEntities()
    {
        // Arrange
        var restaurants = TestDataBuilder.CreateRestaurants(3);
        await _sut.AddRangeAsync(restaurants);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAllAsync();

        // Assert 
        result.Should().HaveCount(3);
    }

    [Fact]
    public async Task UpdateAsync_ShouldMarkAsModifiedInTracker()
    {
        // Arrange
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1, name: "Original");
        await _sut.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        restaurant.Name = "Updated";
        await _sut.UpdateAsync(restaurant);
        await _context.SaveChangesAsync();

        // Assert
        var dbResult = await _context.Restaurants.FindAsync(restaurant.Id);
        dbResult!.Name.Should().Be("Updated");
    }

    [Fact]
    public async Task DeleteAsync_ShouldRemoveFromDatabase()
    {
        // Arrange
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1);
        await _sut.AddAsync(restaurant);
        await _context.SaveChangesAsync();

        // Act
        await _sut.DeleteAsync(restaurant);
        await _context.SaveChangesAsync();

        // Assert
        var dbResult = await _context.Restaurants.FindAsync(restaurant.Id);
        dbResult.Should().BeNull();
    }

    [Fact]
    public async Task GetPagedAsync_ReturnsCorrectPageWithTotalCount()
    {
        // Arrange
        var restaurants = Enumerable.Range(1, 10)
            .Select(i => TestDataBuilder.CreateRestaurant(id: i, name: $"Resto {i:D2}"))
            .ToList();
        await _sut.AddRangeAsync(restaurants);
        await _context.SaveChangesAsync();

        Expression<Func<Restaurant, string>> orderBy = r => r.Name;
        var (items, totalCount) = await _sut.GetPagedAsync(pageNumber: 1, pageSize: 3, orderBy: orderBy, descending: false);

        items.Should().HaveCount(3);
        totalCount.Should().Be(10);
        items.First().Name.Should().Be("Resto 01");
    }

    [Fact]
    public async Task MarkDeleted_ShouldChangeEntityStateToDeleted()
    {
        // Arrange
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1);
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();

        // Act
        _sut.MarkDeleted(restaurant);
        
        // Assert
        _context.Entry(restaurant).State.Should().Be(EntityState.Deleted);
        await _context.SaveChangesAsync();
        (await _context.Restaurants.FindAsync(1)).Should().BeNull();
    }

    [Fact]
    public async Task ListAsync_WithSimpleSpecification_ReturnsFilteredData()
    {
        // Примітка: Цей тест вимагає наявності базової реалізації ISpecification
        // Arrange
        var restaurants = new List<Restaurant>
        {
            TestDataBuilder.CreateRestaurant(id: 1, name: "Apple"),
            TestDataBuilder.CreateRestaurant(id: 2, name: "Banana")
        };
        await _sut.AddRangeAsync(restaurants);
        await _context.SaveChangesAsync();

        var spec = new TestRestaurantSpecification("Apple");

        // Act
        var result = await _sut.ListAsync(spec);

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Apple");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}

public class TestRestaurantSpecification : BaseSpecification<Restaurant>
{
    public TestRestaurantSpecification(string name) 
        : base(r => r.Name == name)
    {
    }
}