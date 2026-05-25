using Orders.Bll.DTOs.OrderDish;

namespace Orders.Bll.DTOs.Order
{
    public class CreateOrderRequestDto
    {
        public int RestaurantId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public List<CreateOrderDishDto> Dishes { get; set; } = new();
    }
}
