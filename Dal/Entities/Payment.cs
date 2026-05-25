using Orders.Dal.Enums;
using System.ComponentModel.DataAnnotations.Schema;

namespace Orders.Dal.Entities
{
    [Table("Payments")]
    public class Payment : BaseEntity
    {
        public int OrderId { get; set; }
        public decimal Amount { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; } = string.Empty;  

        public string? TransactionId { get; set; }

        public Order Order { get; set; } = null!;
    }
}
