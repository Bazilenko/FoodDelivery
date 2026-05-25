using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;


namespace Catalog.Dal.Tests;

public class CategoryRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly CategoryRepository _sut;

    public CategoryRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new CategoryRepository(_context);
    }

    [Fact]
    public async Task GetByRestaurantAsync_ShouldReturnOnlyCategoriesForSpecificRestaurant()
    {
        // Arrange
        var targetRestaurantId = 1;
        var otherRestaurantId = 2;

        var categories = new List<Category>
        {
            TestDataBuilder.CreateCategory(id: 1, name: "Pizza", restaurantId: targetRestaurantId),
            TestDataBuilder.CreateCategory(id: 2, name: "Burgers", restaurantId: targetRestaurantId),
            TestDataBuilder.CreateCategory(id: 3, name: "Sushi", restaurantId: otherRestaurantId)
        };

        await _context.Categories.AddRangeAsync(categories);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByRestaurantAsync(targetRestaurantId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.RestaurantId.Should().Be(targetRestaurantId));
        result.Select(c => c.Name).Should().Contain(new[] { "Pizza", "Burgers" });
        result.Select(c => c.Name).Should().NotContain("Sushi");
    }

    [Fact]
    public async Task GetWithDishesAsync_ShouldReturnCategoryWithIncludedDishes()
    {
        // Arrange
        var restaurantId = 10;
        var category = TestDataBuilder.CreateCategory(id: 5, name: "Main Course", restaurantId: restaurantId);

        var dishes = new List<Dish>
        {
            TestDataBuilder.CreateDish(id: 1, name: "Steak", categoryId: category.Id, restaurantId: restaurantId),
            TestDataBuilder.CreateDish(id: 2, name: "Pasta", categoryId: category.Id, restaurantId: restaurantId)
        };

        await _context.Categories.AddAsync(category);
        await _context.Dishes.AddRangeAsync(dishes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetWithDishesAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Id.Should().Be(category.Id);
        result.Dishes.Should().HaveCount(2);
        result.Dishes.Select(d => d.Name).Should().Contain(new[] { "Steak", "Pasta" });
    }

    [Fact]
    public async Task AddAsync_ShouldPersistCategoryWithRestaurantId()
    {
        // Arrange
        var restaurantId = 7;
        var category = TestDataBuilder.CreateCategory(name: "Drinks", restaurantId: restaurantId);

        // Act
        await _sut.AddAsync(category);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var persisted = await _sut.GetByIdAsync(category.Id);
        persisted.Should().NotBeNull();
        persisted!.Name.Should().Be("Drinks");
        persisted.RestaurantId.Should().Be(restaurantId);
    }

    [Fact]
    public async Task GetWithDishesAsync_WhenCategoryHasNoDishes_ShouldReturnEmptyCollection()
    {
        // Arrange
        var category = TestDataBuilder.CreateCategory(name: "Empty Category", restaurantId: 1);
        await _context.Categories.AddAsync(category);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetWithDishesAsync(category.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Dishes.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}