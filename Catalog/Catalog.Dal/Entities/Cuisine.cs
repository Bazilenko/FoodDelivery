namespace Catalog.Dal.Entities
{
    public class Cuisine : BaseEntity
    {
        public string Name{get; set;}
        public string? ImageUrl{get; set;}
        
        public ICollection<RestaurantCuisine> RestaurantCuisines {get; set;} = [];
    }
}