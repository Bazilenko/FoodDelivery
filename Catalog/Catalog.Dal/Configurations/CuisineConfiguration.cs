using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class CuisineConfiguration : IEntityTypeConfiguration<Cuisine>
    {

        public void Configure(EntityTypeBuilder<Cuisine>builder)
        {
            builder.HasKey(c => c.Id);

            builder.Property(c => c.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(c => c.ImageUrl)
                .IsRequired()
                .HasMaxLength(500);

            builder.HasQueryFilter(c => !c.IsDeleted);

            builder.HasMany(c => c.RestaurantCuisines)
                .WithOne(rc => rc.Cuisine)
                .HasForeignKey(rc => rc.CuisineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
