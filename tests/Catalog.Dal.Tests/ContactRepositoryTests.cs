using FluentAssertions;
using Catalog.Dal.Entities;
using Catalog.Dal.Repositories;
using Catalog.Dal.Context;
using Catalog.Dal.Enums;


namespace Catalog.Dal.Tests;

public class ContactRepositoryTests : IDisposable
{
    private readonly MyDbContext _context;
    private readonly ContactRepository _sut;

    public ContactRepositoryTests()
    {
        _context = DbContextFactory.Create();
        _sut = new ContactRepository(_context);
    }

    [Fact]
    public async Task GetByRestaurantAsync_ShouldReturnOnlyContactsForTargetRestaurant()
    {
        // Arrange
        int targetId = 1;
        int otherId = 2;
        var contacts = new List<Contact>
        {
            TestDataBuilder.CreateContact(id: 1, restaurantId: targetId, value: "target-1"),
            TestDataBuilder.CreateContact(id: 2, restaurantId: targetId, value: "target-2"),
            TestDataBuilder.CreateContact(id: 3, restaurantId: otherId, value: "other")
        };
        await _context.Contacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByRestaurantAsync(targetId);

        // Assert
        result.Should().HaveCount(2);
        result.Should().AllSatisfy(c => c.RestaurantId.Should().Be(targetId));
        result.Select(c => c.Value).Should().Contain(new[] { "target-1", "target-2" });
    }

    [Fact]
    public async Task GetByTypeAsync_ShouldReturnOnlySpecificTypeForRestaurant()
    {
        // Arrange
        int restaurantId = 10;
        var contacts = new List<Contact>
        {
            TestDataBuilder.CreateContact(restaurantId: restaurantId, type: ContactType.Phone, value: "123"),
            TestDataBuilder.CreateContact(restaurantId: restaurantId, type: ContactType.Email, value: "test@test.com"),
            TestDataBuilder.CreateContact(restaurantId: 99, type: ContactType.Phone, value: "456") // Інший ресторан
        };
        await _context.Contacts.AddRangeAsync(contacts);
        await _context.SaveChangesAsync();

        // Act
        var result = await _sut.GetByTypeAsync(restaurantId, ContactType.Phone);

        // Assert
        result.Should().HaveCount(1);
        result.Single().Value.Should().Be("123");
        result.Single().Type.Should().Be(ContactType.Phone);
    }

    [Fact]
    public async Task AddAsync_ValidContact_ShouldPersistWithEnumType()
    {
        // Arrange
        var contact = TestDataBuilder.CreateContact(value: "insta_page", type: ContactType.Website);

        // Act
        await _sut.AddAsync(contact);
        await _context.SaveChangesAsync();

        _context.ChangeTracker.Clear();

        // Assert
        var result = await _sut.GetByIdAsync(contact.Id);
        result.Should().NotBeNull();
        result!.Type.Should().Be(ContactType.Website);
        result.Value.Should().Be("insta_page");
    }

    [Fact]
    public async Task UpdateAsync_ExistingContact_ShouldChangeValueAndType()
    {
        // Arrange
        var contact = TestDataBuilder.CreateContact(type: ContactType.Email, value: "old@mail.com");
        await _context.AddAsync(contact);
        await _context.SaveChangesAsync();
        _context.ChangeTracker.Clear();

        // Act
        contact.Value = "new@mail.com";
        contact.Type = ContactType.Website;
        await _sut.UpdateAsync(contact);
        await _context.SaveChangesAsync();

        // Assert
        var updated = await _context.Contacts.FindAsync(contact.Id);
        updated!.Value.Should().Be("new@mail.com");
        updated.Type.Should().Be(ContactType.Website);
    }

    [Fact]
    public async Task GetByRestaurantAsync_WhenEmpty_ShouldReturnEmptyCollection()
    {
        // Act
        var result = await _sut.GetByRestaurantAsync(404);

        // Assert
        result.Should().BeEmpty();
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }
}