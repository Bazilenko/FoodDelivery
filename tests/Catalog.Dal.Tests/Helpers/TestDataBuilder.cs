using Catalog.Dal.Entities;
using Catalog.Dal.Enums;

public static class TestDataBuilder
{
    public static Restaurant CreateRestaurant(
        int id = 1,
        string name = "Test Restaurant",
        decimal rating = 4.5m,
        decimal deliveryRadius = 5.0m)
    {
        return new Restaurant
        {
            Id = id,
            Name = name,
            Rating = rating,
            DeliveryRadiusKm = deliveryRadius,
            ImageUrl = "https://example.com/logo.png",
            Description = "Test description",
            Addresses = new List<Address>(),
            Contacts = new List<Contact>(),
            Dishes = new List<Dish>(),
            Categories = new List<Category>(),
            WorkingHours = new List<WorkingHour>(),
            RestaurantCuisines = new List<RestaurantCuisine>()
        };
    }

    public static Address CreateAddress(
        int id = 1,
        int restaurantId = 1,
        string street = "Main St",
        string city = "Kyiv") =>
        new Address
        {
            Id = id,
            RestaurantId = restaurantId,
            City = city,
            Street = street,
            BuildingNumber = "10A",
            PostalCode = "01001",
            Latitude = 50.45m,
            Longitude = 30.52m
        };

    public static Contact CreateContact(
        int id = 1,
        int restaurantId = 1,
        ContactType type = ContactType.Phone,
        string value = "+380000000000") =>
        new Contact
        {
            Id = id,
            RestaurantId = restaurantId,
            Type = type,
            Value = value
        };

    public static WorkingHour CreateWorkingHour(
        int id = 1,
        int restaurantId = 1,
        int day = 1) =>
        new WorkingHour
        {
            Id = id,
            RestaurantId = restaurantId,
            DayOfWeek = day,
            OpeningTime = new TimeSpan(9, 0, 0),
            ClosingTime = new TimeSpan(22, 0, 0),
            IsClosed = false
        };

    public static Category CreateCategory(
        int id = 1, 
        int restaurantId = 1, 
        string name = "Pizza") =>
        new Category
        {
            Id = id,
            Name = name,
            RestaurantId = restaurantId
        };

    public static Dish CreateDish(
        int id = 1,
        int restaurantId = 1,
        int categoryId = 1,
        string name = "Margherita") =>
        new Dish
        {
            Id = id,
            RestaurantId = restaurantId,
            CategoryId = categoryId,
            Name = name,
            Description = "Classic pizza",
            Price = 250m,
            IsAvailable = true,
            ImageUrl = "pizza.png",
            ModifierGroups = new List<ModifierGroup>()
        };

    public static ModifierGroup CreateModifierGroup(
        int id = 1, 
        int dishId = 1, 
        string name = "Extra Toppings") =>
        new ModifierGroup
        {
            Id = id,
            DishId = dishId,
            Name = name,
            MinSelect = 0,
            MaxSelect = 5,
            DishOptions = new List<DishOption>()
        };

    public static DishOption CreateDishOption(
        int id = 1, 
        int groupId = 1, 
        decimal price = 35m,
        string name = "Extra Cheese") =>
        new DishOption
        {
            Id = id,
            ModifierGroupId = groupId,
            Name = name,
            Price = price,
            IsAvailable = true
        };

    public static Cuisine CreateCuisine(int id = 1, string name = "Italian") =>
        new Cuisine
        {
            Id = id,
            Name = name,
            ImageUrl = "cuisine.png"
        };

    public static RestaurantCuisine CreateRestaurantCuisine(int restaurantId, int cuisineId) =>
        new RestaurantCuisine
        {
            RestaurantId = restaurantId,
            CuisineId = cuisineId
        };

    public static List<Restaurant> CreateRestaurants(int count = 3) =>
        Enumerable.Range(1, count)
            .Select(i => CreateRestaurant(id: i, name: $"Restaurant {i}"))
            .ToList();
}