using System.ComponentModel.DataAnnotations.Schema;
namespace Orders.Dal.Entities
{
    [Table("OrderDishOptions")]
    public class OrderDishOption : BaseEntity
    {
        public int OrderDishId { get; set; }
        public int OptionId { get; set; }

        public string OptionNameSnapshot { get; set; } = string.Empty;
        public decimal PriceAtTimeOfOrder { get; set; }

        public OrderDish OrderDish { get; set; } = null!;
    }
}