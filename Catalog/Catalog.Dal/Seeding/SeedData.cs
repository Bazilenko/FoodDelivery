using Bogus;
using Catalog.Dal.Entities;
using Catalog.Dal.Enums;
using Catalog.Dal.UOW.Interfaces;

public static class SeedData
{
    public static async Task SeedAsync(IUnitOfWork uow)
    {
        var existingRestaurants = await uow.Restaurants.GetAllAsync();
        if (existingRestaurants.Any()) return;

        var faker = new Faker();

        var cuisineFaker = new Faker<Cuisine>()
            .RuleFor(c => c.Name, f => f.Address.Country() + " Cuisine")
            .RuleFor(c => c.ImageUrl, f => f.Image.PicsumUrl());

        var cuisines = cuisineFaker.Generate(5);
        await uow.Cuisines.AddRangeAsync(cuisines);
        await uow.SaveChangesAsync();

        var restaurantFaker = new Faker<Restaurant>()
            .RuleFor(r => r.Name, f => f.Company.CompanyName())
            .RuleFor(r => r.Description, f => f.Lorem.Sentence(10))
            .RuleFor(r => r.ImageUrl, f => f.Image.PicsumUrl())
            .RuleFor(r => r.Rating, f => f.Random.Decimal(3.5m, 5m))
            .RuleFor(r => r.DeliveryRadiusKm, f => f.Random.Decimal(1, 15))
            .RuleFor(r => r.CreatedAt, f => DateTime.UtcNow);

        var restaurants = restaurantFaker.Generate(5);
        await uow.Restaurants.AddRangeAsync(restaurants);
        await uow.SaveChangesAsync(); 

        foreach (var rest in restaurants)
        {
            var address = new Address
            {
                City = faker.Address.City(),
                Street = faker.Address.StreetName(),
                BuildingNumber = faker.Address.BuildingNumber(),
                Latitude = (decimal)faker.Address.Latitude(),
                Longitude = (decimal)faker.Address.Longitude(),
                RestaurantId = rest.Id
            };
            await uow.Addresses.AddAsync(address);

            var contactFaker = new Faker<Contact>()
                .RuleFor(c => c.Type, f => f.PickRandom<ContactType>())
                .RuleFor(c => c.Value, (f, c) => c.Type == ContactType.Phone ? f.Phone.PhoneNumber() : f.Internet.Email())
                .RuleFor(c => c.RestaurantId, rest.Id);
            
            await uow.Contacts.AddRangeAsync(contactFaker.Generate(2));

            for (int i = 0; i < 7; i++)
            {
                await uow.WorkingHours.AddAsync(new WorkingHour
                {
                    DayOfWeek = i,
                    OpeningTime = new TimeSpan(9, 0, 0),
                    ClosingTime = new TimeSpan(22, 0, 0),
                    RestaurantId = rest.Id
                });
            }

            var categoryFaker = new Faker<Category>()
                .RuleFor(c => c.Name, f => f.PickRandom("Burgers", "Pizza", "Salads", "Drinks", "Desserts"))
                .RuleFor(c => c.RestaurantId, rest.Id);

            var categories = categoryFaker.Generate(3);
            await uow.Categories.AddRangeAsync(categories);
            await uow.SaveChangesAsync(); 

            foreach (var cat in categories)
            {
                var dishFaker = new Faker<Dish>()
                    .RuleFor(d => d.Name, f => f.Commerce.ProductName())
                    .RuleFor(d => d.Price, f => f.Random.Decimal(100, 500))
                    .RuleFor(d => d.CategoryId, cat.Id)
                    .RuleFor(d => d.RestaurantId, rest.Id)
                    .RuleFor(d => d.IsAvailable, true);

                var dishes = dishFaker.Generate(4);
                await uow.Dishes.AddRangeAsync(dishes);
                await uow.SaveChangesAsync();

                foreach (var dish in dishes)
                {
                    var modGroup = new ModifierGroup
                    {
                        Name = "Extra toppings",
                        MinSelect = 0,
                        MaxSelect = 3,
                        DishId = dish.Id
                    };
                    await uow.ModifierGroups.AddAsync(modGroup);
                    await uow.SaveChangesAsync();

                    var optionFaker = new Faker<DishOption>()
                        .RuleFor(o => o.Name, f => f.Commerce.ProductMaterial())
                        .RuleFor(o => o.Price, f => f.Random.Decimal(10, 50))
                        .RuleFor(o => o.ModifierGroupId, modGroup.Id);

                    await uow.DishOptions.AddRangeAsync(optionFaker.Generate(3));
                }
            }

            var restaurantCuisine = new RestaurantCuisine
            {
                RestaurantId = rest.Id,
                CuisineId = faker.PickRandom(cuisines).Id
            };
            await uow.RestaurantCuisines.AddAsync(restaurantCuisine);
        }

        await uow.SaveChangesAsync();
    }
}