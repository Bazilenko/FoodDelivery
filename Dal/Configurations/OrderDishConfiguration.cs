using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orders.Dal.Configurations
{
    public class OrderDishConfiguration : IEntityTypeConfiguration<OrderDish>
    {
        public void Configure(EntityTypeBuilder<OrderDish> builder)
        {
            builder.ToTable("OrderDishes");
            builder.HasKey(od => od.Id);

            builder.Property(od => od.DishNameSnapshot)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(od => od.CategorySnapshot)
                .HasMaxLength(100);

            builder.Property(od => od.Quantity)
                .IsRequired();

            builder.Property(od => od.PriceAtTimeOfOrder)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.HasQueryFilter(od => !od.IsDeleted);

            builder.HasOne(od => od.Order)
                .WithMany(o => o.OrderDishes)
                .HasForeignKey(od => od.OrderId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(od => od.OrderDishOptions)
                .WithOne(odo => odo.OrderDish)
                .HasForeignKey(odo => odo.OrderDishId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}