using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Dal.Entities
{
    [Table("OrderDishes")]
    public class OrderDish : BaseEntity
    {
        public int OrderId { get; set; }
        public int DishId { get; set; }           

        public string DishNameSnapshot { get; set; } = string.Empty;
        public string? CategorySnapshot { get; set; }

        public int Quantity { get; set; }
        public decimal PriceAtTimeOfOrder { get; set; }

        public Order Order { get; set; } = null!;
        public ICollection<OrderDishOption> OrderDishOptions { get; set; } = [];
    }
}
