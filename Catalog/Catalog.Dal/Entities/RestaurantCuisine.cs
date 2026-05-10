namespace Catalog.Dal.Entities
{
    public class RestaurantCuisine : BaseEntity
    {
        public int RestaurantId {get; set;}
        public int CuisineId {get; set;}

        public Restaurant Restaurant {get; set;} = null!;
        public Cuisine Cuisine {get; set;} = null!;
    }
}