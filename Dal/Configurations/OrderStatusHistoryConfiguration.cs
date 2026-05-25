using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orders.Dal.Configurations
{
    public class OrderStatusHistoryConfiguration : IEntityTypeConfiguration<OrderStatusHistory>
    {
        public void Configure(EntityTypeBuilder<OrderStatusHistory> builder)
        {
            builder.ToTable("OrderStatusHistory");
            builder.HasKey(h => h.Id);

            builder.Property(h => h.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(h => h.Comment)
                .HasMaxLength(500);

            builder.HasIndex(h => h.OrderId);

            builder.HasQueryFilter(h => !h.IsDeleted);

            builder.HasOne(h => h.Order)
                .WithMany(o => o.StatusHistory)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}