using Orders.Dal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Dal.Entities
{
    [Table("OrderStatusHistory")]
    public class OrderStatusHistory : BaseEntity
    {
        public int OrderId { get; set; }
        public OrderStatus Status { get; set; }
        public string? Comment { get; set; }    

        public Order Order { get; set; } = null!;
    }
}