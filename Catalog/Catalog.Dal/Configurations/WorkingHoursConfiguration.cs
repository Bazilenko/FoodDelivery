using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class WorkingHoursConfiguration : IEntityTypeConfiguration<WorkingHour>
    {

        public void Configure(EntityTypeBuilder<WorkingHour> builder)
        {
            builder.HasKey(wh => wh.Id);

             builder.Property(w => w.DayOfWeek)
            .IsRequired();

            builder.Property(w => w.OpeningTime)
            .IsRequired();
 
             builder.Property(w => w.ClosingTime)
            .IsRequired();
 
            builder.Property(w => w.IsClosed)
            .IsRequired();
 
            builder.HasIndex(w => new { w.DayOfWeek, w.RestaurantId })
            .IsUnique();

            builder.HasQueryFilter(w => !w.IsDeleted);

            builder.HasOne(w => w.Restaurant)
            .WithMany(r => r.WorkingHours)
            .HasForeignKey(w => w.RestaurantId)
            .OnDelete(DeleteBehavior.Cascade);
        }

    }
}