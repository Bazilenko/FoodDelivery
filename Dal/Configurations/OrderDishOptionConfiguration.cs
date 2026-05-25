using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orders.Dal.Configurations
{
    public class OrderDishOptionConfiguration : IEntityTypeConfiguration<OrderDishOption>
    {
        public void Configure(EntityTypeBuilder<OrderDishOption> builder)
        {
            builder.ToTable("OrderDishOptions");
            builder.HasKey(odo => odo.Id);

            builder.Property(odo => odo.OptionNameSnapshot)
                .IsRequired()
                .HasMaxLength(200);

            builder.Property(odo => odo.PriceAtTimeOfOrder)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.HasQueryFilter(odo => !odo.IsDeleted);

            builder.HasOne(odo => odo.OrderDish)
                .WithMany(od => od.OrderDishOptions)
                .HasForeignKey(odo => odo.OrderDishId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}