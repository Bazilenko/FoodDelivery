using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;


namespace Catalog.Dal.Tests;

public class RestaurantRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly RestaurantRepository _sut;

    public RestaurantRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new RestaurantRepository(_context);
    }

    [Fact]
    public async Task GetWithFullDetailsAsync_ShouldReturnEverythingInOneQuery()
    {
        // Arrange
        var cuisine = TestDataBuilder.CreateCuisine(id: 1, name: "Ukrainian");
        var restaurant = TestDataBuilder.CreateRestaurant(id: 1, name: "Full Details Resto");
        
        restaurant.Addresses.Add(TestDataBuilder.CreateAddress(restaurantId: 1, city: "Kyiv"));
        restaurant.Contacts.Add(TestDataBuilder.CreateContact(restaurantId: 1, value: "0991112233"));
        restaurant.WorkingHours.Add(TestDataBuilder.CreateWorkingHour(restaurantId: 1, day: 1));
        restaurant.RestaurantCuisines.Add(new RestaurantCuisine { RestaurantId = 1, CuisineId = 1 });

        await _context.Cuisines.AddAsync(cuisine);
        await _context.Restaurants.AddAsync(restaurant);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetWithFullDetailsAsync(restaurant.Id);

        // Assert
        result.Should().NotBeNull();
        result!.Addresses.Should().NotBeEmpty();
        result.Contacts.Should().NotBeEmpty();
        result.WorkingHours.Should().NotBeEmpty();
        result.RestaurantCuisines.Should().NotBeEmpty();
        result.RestaurantCuisines.First().Cuisine.Name.Should().Be("Ukrainian"); 
    }

    [Fact]
    public async Task GetByCityAsync_ShouldFilterByAddressCollection()
    {
        // Arrange
        var r1 = TestDataBuilder.CreateRestaurant(id: 1, name: "Kyiv Palace");
        r1.Addresses.Add(TestDataBuilder.CreateAddress(restaurantId: 1, city: "Kyiv"));

        var r2 = TestDataBuilder.CreateRestaurant(id: 2, name: "Lviv Hub");
        r2.Addresses.Add(TestDataBuilder.CreateAddress(restaurantId: 2, city: "Lviv"));

        await _context.Restaurants.AddRangeAsync(r1, r2);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByCityAsync("Kyiv");

        // Assert
        result.Should().HaveCount(1);
        result.First().Name.Should().Be("Kyiv Palace");
        result.First().Addresses.Should().NotBeEmpty();
    }

    [Fact]
    public async Task GetPagedByCuisineAsync_ShouldReturnCorrectPageAndTotalCount()
    {
        // Arrange
        int cuisineId = 5;
        var cuisine = TestDataBuilder.CreateCuisine(id: cuisineId);
        await _context.Cuisines.AddAsync(cuisine);

        var restaurants = Enumerable.Range(1, 5).Select(i => 
        {
            var r = TestDataBuilder.CreateRestaurant(id: i, name: $"Resto {i}");
            r.RestaurantCuisines.Add(new RestaurantCuisine { RestaurantId = i, CuisineId = cuisineId });
            return r;
        }).ToList();

        await _context.Restaurants.AddRangeAsync(restaurants);
        await _context.SaveChangesAsync();

        // Act
        var (items, totalCount) = await _sut.GetPagedByCuisineAsync(cuisineId, pageNumber: 1, pageSize: 3);

        // Assert
        totalCount.Should().Be(5);
        items.Should().HaveCount(3);
        items.First().RestaurantCuisines.First().Cuisine.Should().NotBeNull();
    }

    [Fact]
    public async Task GetByCuisineAsync_ShouldIncludeAddresses()
    {
        // Arrange
        int cuisineId = 10;
        var r1 = TestDataBuilder.CreateRestaurant(id: 1);
        r1.Addresses.Add(TestDataBuilder.CreateAddress(restaurantId: 1));
        r1.RestaurantCuisines.Add(new RestaurantCuisine { RestaurantId = 1, CuisineId = cuisineId });

        await _context.Restaurants.AddAsync(r1);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        var result = await _sut.GetByCuisineAsync(cuisineId);

        // Assert
        result.Should().NotBeEmpty();
        result.First().Addresses.Should().NotBeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}