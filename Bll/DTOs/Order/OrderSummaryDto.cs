using Orders.Dal.Enums;

namespace Bll.DTOs.Order
{
    public class OrderSummaryDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public int RestaurantId { get; set; }

        public decimal TotalAmount { get; set; }
        public decimal DeliveryFee { get; set; }

        public string DeliveryAddress { get; set; } = string.Empty;
        public string? Notes { get; set; }

        public OrderStatus Status { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}