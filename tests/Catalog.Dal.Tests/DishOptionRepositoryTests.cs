using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;

namespace Catalog.Dal.Tests;

public class DishOptionRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly DishOptionRepository _sut;

    public DishOptionRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new DishOptionRepository(_context);
    }

    [Fact]
    public async Task GetByModifierGroupAsync_ShouldReturnAllOptionsForGroup()
    {
        // Arrange
        var groupId = 1;
        var options = new List<DishOption>
        {
            TestDataBuilder.CreateDishOption(id: 1, groupId: groupId, name: "Salami"),
            TestDataBuilder.CreateDishOption(id: 2, groupId: groupId, name: "Olives"),
            TestDataBuilder.CreateDishOption(id: 3, groupId: 99, name: "Other Group Option")
        };

        await _context.DishesOptions.AddRangeAsync(options);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByModifierGroupAsync(groupId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.ModifierGroupId.Should().Be(groupId));
        result.Select(o => o.Name).Should().Contain(new[] { "Salami", "Olives" });
    }

    [Fact]
    public async Task GetAvailableByModifierGroupAsync_ShouldReturnOnlyAvailableOptions()
    {
        // Arrange
        var groupId = 10;
        var options = new List<DishOption>
        {
            new DishOption { Id = 1, ModifierGroupId = groupId, Name = "Available", IsAvailable = true, Price = 10 },
            new DishOption { Id = 2, ModifierGroupId = groupId, Name = "Not Available", IsAvailable = false, Price = 10 },
            new DishOption { Id = 3, ModifierGroupId = groupId, Name = "Also Available", IsAvailable = true, Price = 15 }
        };

        await _context.DishesOptions.AddRangeAsync(options);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAvailableByModifierGroupAsync(groupId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(o => o.IsAvailable.Should().BeTrue());
        result.Select(o => o.Name).Should().Contain("Available", "Also Available");
    }

    [Fact]
    public async Task AddAsync_ShouldPersistCorrectData()
    {
        // Arrange
        var groupId = 5;
        var option = TestDataBuilder.CreateDishOption(name: "Extra Cheese", price: 30.0m, groupId: groupId);

        // Act
        await _sut.AddAsync(option);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var result = await _sut.GetByIdAsync(option.Id);
        result.Should().NotBeNull();
        result!.Name.Should().Be("Extra Cheese");
        result.Price.Should().Be(30.0m);
        result.ModifierGroupId.Should().Be(groupId);
    }

    [Fact]
    public async Task GetByIdAsync_WhenModifierGroupIsDeleted_ShouldHandleCascade()
    {
        // Arrange
        var dish = TestDataBuilder.CreateDish();
        await _context.Dishes.AddAsync(dish);
        await _context.SaveChangesAsync();

        var group = TestDataBuilder.CreateModifierGroup(dishId: dish.Id);
        await _context.ModifierGroups.AddAsync(group);
        await _context.SaveChangesAsync();

        var option = TestDataBuilder.CreateDishOption(groupId: group.Id);
        await _sut.AddAsync(option);
        await _context.SaveChangesAsync();

        // Act
        _context.ModifierGroups.Remove(group);
        await _context.SaveChangesAsync();

        // Assert
        var result = await _sut.GetByIdAsync(option.Id);
        result.Should().BeNull();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}