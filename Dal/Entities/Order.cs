using Orders.Dal.Enums;

namespace Orders.Dal.Entities
{
    public class Order : BaseEntity
    {
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }
        public string DeliveryAddress { get; set; } = string.Empty;

        public string? Notes { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalAmount { get; set; }

        public OrderStatus Status { get; set; }

        public ICollection<OrderDish> OrderDishes { get; set; } = [];
        public ICollection<OrderStatusHistory> StatusHistory { get; set; } = [];
        public Payment? Payment { get; set; }
    }
}
