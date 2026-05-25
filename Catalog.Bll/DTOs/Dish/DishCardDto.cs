namespace Catalog.Bll.DTOs.Dish
{
    public class DishCardDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string? ImageUrl { get; set; }
        public decimal Price { get; set; }
        public decimal? Calories { get; set; }
        public bool IsAvailable { get; set; }
    }
}
