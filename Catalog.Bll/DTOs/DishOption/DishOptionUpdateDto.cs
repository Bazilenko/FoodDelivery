namespace Catalog.Bll.DTOs.DishOption
{
    public class DishOptionUpdateDto
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public bool IsAvailable { get; set; }
    }
}
