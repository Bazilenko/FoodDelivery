using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace Catalog.Dal.Tests;

public class AddressRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly AddressRepository _sut;

    public AddressRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new AddressRepository(_context);
    }

    [Fact]
    public async Task GetAddressesByCityAsync_ShouldReturnCorrectAddresses()
    {
        // Arrange
        var addresses = new List<Address>
        {
            TestDataBuilder.CreateAddress(city: "Kyiv"),
            TestDataBuilder.CreateAddress(city: "Kyiv"),
            TestDataBuilder.CreateAddress(city: "Lviv")
        };
        await _context.Addresses.AddRangeAsync(addresses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAddressesByCityAsync("Kyiv");

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.City.Should().Be("Kyiv"));
    }

    [Fact]
    public async Task GetAddressesByRestaurantIdAsync_MultipleAddresses_ReturnsAllMatching()
    {
        // Arrange
        int targetRestaurantId = 10;
        var addresses = new List<Address>
        {
            TestDataBuilder.CreateAddress(restaurantId: targetRestaurantId, street: "First Street"),
            TestDataBuilder.CreateAddress(restaurantId: targetRestaurantId, street: "Second Street"),
            TestDataBuilder.CreateAddress(restaurantId: 999, street: "Other Restaurant Street")
        };
        await _context.Addresses.AddRangeAsync(addresses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetAddressesByRestaurantIdAsync(targetRestaurantId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.RestaurantId.Should().Be(targetRestaurantId));
        result.Select(a => a.Street).Should().Contain(new[] { "First Street", "Second Street" });
    }

    [Fact]
    public async Task AddAsync_ShouldPersistDataWithCorrectPostalCode()
    {
        // Arrange
        var address = TestDataBuilder.CreateAddress(street: "Test Avenue");

        // Act
        await _sut.AddAsync(address);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var persisted = await _sut.GetByIdAsync(address.Id);
        persisted.Should().NotBeNull();
        persisted!.Street.Should().Be("Test Avenue");
        persisted.PostalCode.Should().Be("01001");
        persisted.BuildingNumber.Should().Be("10"); 
    }

    [Fact]
    public async Task GetByRestaurantIdAsync_WhenNoAddressesExist_ReturnsEmpty()
    {
        // Act
        var result = await _sut.GetAddressesByRestaurantIdAsync(404);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}