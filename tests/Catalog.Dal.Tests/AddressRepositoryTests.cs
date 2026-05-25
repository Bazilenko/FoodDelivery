using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;


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
    public async Task GetByRestaurantAsync_ShouldReturnAddressesOnlyForSpecificRestaurant()
    {
        // Arrange
        int targetRestaurantId = 1;
        int otherRestaurantId = 2;

        var addresses = new List<Address>
        {
            TestDataBuilder.CreateAddress(id: 1, restaurantId: targetRestaurantId, city: "Kyiv"),
            TestDataBuilder.CreateAddress(id: 2, restaurantId: targetRestaurantId, city: "Lviv"),
            TestDataBuilder.CreateAddress(id: 3, restaurantId: otherRestaurantId, city: "Odesa")
        };

        await _context.Addresses.AddRangeAsync(addresses);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByRestaurantAsync(targetRestaurantId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(a => a.RestaurantId.Should().Be(targetRestaurantId));
        result.Select(a => a.City).Should().Contain(new[] { "Kyiv", "Lviv" });
        result.Select(a => a.City).Should().NotContain("Odesa");
    }

    [Fact]
    public async Task AddAsync_ShouldAddAddressToChangeTracker()
    {
        // Arrange
        var address = TestDataBuilder.CreateAddress(city: "Dnipro", street: "Central St");

        // Act
        await _sut.AddAsync(address);
        await _context.SaveChangesAsync(); 

        _context.ChangeTracker.Clear();

        // Assert
        var persisted = await _context.Addresses.FindAsync(address.Id);
        persisted.Should().NotBeNull();
        persisted!.City.Should().Be("Dnipro");
        persisted.Street.Should().Be("Central St");
    }

    [Fact]
    public async Task GetByRestaurantAsync_WhenNoAddressesExist_ReturnsEmpty()
    {
        // Act
        var result = await _sut.GetByRestaurantAsync(999);

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateAsync_ShouldModifyExistingAddress()
    {
        // Arrange
        var address = TestDataBuilder.CreateAddress(city: "Kharkiv");
        await _context.Addresses.AddAsync(address);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        address.City = "Poltava";
        await _sut.UpdateAsync(address);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Addresses.FindAsync(address.Id);
        updated!.City.Should().Be("Poltava");
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}