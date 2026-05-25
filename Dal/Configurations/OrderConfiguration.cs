using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orders.Dal.Configurations
{
    public class OrderConfiguration : IEntityTypeConfiguration<Order>
    {
        public void Configure(EntityTypeBuilder<Order> builder)
        {
            builder.ToTable("Orders");
            builder.HasKey(o => o.Id);

            builder.Property(o => o.DeliveryAddress)
                .IsRequired()
                .HasMaxLength(500);

            builder.Property(o => o.Notes)
                .HasMaxLength(1000);

            builder.Property(o => o.DeliveryFee)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(o => o.TotalAmount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(o => o.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.HasQueryFilter(o => !o.IsDeleted);

            builder.HasMany(o => o.OrderDishes)
                .WithOne(od => od.Order)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(o => o.StatusHistory)
                .WithOne(h => h.Order)
                .HasForeignKey(h => h.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasOne(o => o.Payment)
                .WithOne(p => p.Order)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}