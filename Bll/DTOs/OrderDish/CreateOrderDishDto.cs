using Orders.Bll.DTOs.OrderDishOption;

namespace Orders.Bll.DTOs.OrderDish
{
    public class CreateOrderDishDto
    {
        public int DishId { get; set; }
        public string DishNameSnapshot { get; set; } = string.Empty;
        public string CategorySnapshot { get; set; } = string.Empty;
        public int Quantity { get; set; }
        public decimal PriceAtTimeOfOrder { get; set; }

        public List<CreateOrderDishOptionDto> Options { get; set; } = new();
    }
}
