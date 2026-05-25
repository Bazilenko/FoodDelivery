namespace Catalog.Bll.DTOs.DishOption
{
    public class DishOptionCreateDto
    {
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; } = true;
    }
}
