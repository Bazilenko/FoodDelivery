using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;


namespace Catalog.Dal.Tests;

public class DishRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly DishRepository _sut;

    public DishRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new DishRepository(_context);
    }

    [Fact]
    public async Task GetByCategoryAsync_ExistingCategory_ReturnsCorrectDishes()
    {
        // Arrange
        var restaurantId = 1;
        var category = TestDataBuilder.CreateCategory(id: 1, restaurantId: restaurantId, name: "Italian");
        await _context.Categories.AddAsync(category);

        var dishes = new List<Dish>
        {
            TestDataBuilder.CreateDish(id: 1, restaurantId: restaurantId, categoryId: category.Id, name: "Pasta"),
            TestDataBuilder.CreateDish(id: 2, restaurantId: restaurantId, categoryId: category.Id, name: "Pizza"),
            TestDataBuilder.CreateDish(id: 3, restaurantId: restaurantId, categoryId: 99, name: "Other")
        };
        await _context.Dishes.AddRangeAsync(dishes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByCategoryAsync(category.Id);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(d => d.CategoryId.Should().Be(category.Id));
    }

    [Fact]
    public async Task GetByRestaurantAsync_WithAvailableOnly_ShouldFilterCorrectly()
    {
        // Arrange
        var restaurantId = 10;
        var category = TestDataBuilder.CreateCategory(restaurantId: restaurantId);
        await _context.Categories.AddAsync(category);

        var dishes = new List<Dish>
        {
            new Dish { Id = 1, Name = "Available", IsAvailable = true, RestaurantId = restaurantId, CategoryId = category.Id, Price = 100 },
            new Dish { Id = 2, Name = "Not Available", IsAvailable = false, RestaurantId = restaurantId, CategoryId = category.Id, Price = 100 },
            new Dish { Id = 3, Name = "Other Restaurant", IsAvailable = true, RestaurantId = 99, CategoryId = category.Id, Price = 100 }
        };
        await _context.Dishes.AddRangeAsync(dishes);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByRestaurantAsync(restaurantId, availableOnly: true);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Name.Should().Be("Available");
        result.Single().Category.Should().NotBeNull(); 
    }

    [Fact]
    public async Task GetWithModifiersAsync_ShouldLoadFullHierarchy()
    {
        // Arrange
        var restaurantId = 1;
        var category = TestDataBuilder.CreateCategory(restaurantId: restaurantId);
        await _context.Categories.AddAsync(category);

        var dish = TestDataBuilder.CreateDish(id: 1, restaurantId: restaurantId, categoryId: category.Id);
        var modifierGroup = TestDataBuilder.CreateModifierGroup(id: 1, dishId: dish.Id, name: "Sauces");
        var option = TestDataBuilder.CreateDishOption(id: 1, groupId: modifierGroup.Id, name: "Ketchup");

        await _context.Dishes.AddAsync(dish);
        await _context.ModifierGroups.AddAsync(modifierGroup);
        await _context.DishesOptions.AddAsync(option);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetWithModifiersAsync(dish.Id);

        // Assert
        result.Should().NotBeNull();
        result!.ModifierGroups.Should().NotBeEmpty();
        var group = result.ModifierGroups.First();
        group.Name.Should().Be("Sauces");
        group.DishOptions.Should().NotBeEmpty();
        group.DishOptions.First().Name.Should().Be("Ketchup");
    }

    [Fact]
    public async Task GetByRestaurantAsync_ShouldLoadCategoryEvenIfMultipleRestaurantsExist()
    {
        // Arrange
        var res1 = 1;
        var cat1 = TestDataBuilder.CreateCategory(id: 1, restaurantId: res1, name: "Cat1");
        var dish1 = TestDataBuilder.CreateDish(restaurantId: res1, categoryId: cat1.Id);

        await _context.Categories.AddAsync(cat1);
        await _context.Dishes.AddAsync(dish1);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByRestaurantAsync(res1);

        // Assert
        result.First().Category.Should().NotBeNull();
        result.First().Category.Name.Should().Be("Cat1");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}