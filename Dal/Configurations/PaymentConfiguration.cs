using Microsoft.EntityFrameworkCore;
using Orders.Dal.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Orders.Dal.Configurations
{
    public class PaymentConfiguration : IEntityTypeConfiguration<Payment>
    {
        public void Configure(EntityTypeBuilder<Payment> builder)
        {
            builder.ToTable("Payments");
            builder.HasKey(p => p.Id);

            builder.Property(p => p.Amount)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.Property(p => p.Status)
                .IsRequired()
                .HasConversion<string>()
                .HasMaxLength(50);

            builder.Property(p => p.PaymentMethod)
                .IsRequired()
                .HasMaxLength(50);

            builder.Property(p => p.TransactionId)
                .HasMaxLength(200)
                .IsRequired(false);

            builder.HasIndex(p => p.OrderId)
                .IsUnique();

            builder.HasQueryFilter(p => !p.IsDeleted);

            builder.HasOne(p => p.Order)
                .WithOne(o => o.Payment)
                .HasForeignKey<Payment>(p => p.OrderId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}