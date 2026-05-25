namespace Orders.Bll.DTOs.OrderDish
{
    public class OrderDishDto
    {
        public string DishName { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal Price { get; set; }
        public List<string> AppliedOptions { get; set; } = [];
    }
}
