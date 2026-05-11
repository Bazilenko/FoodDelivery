using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class RestaurantCuisineConfiguration : IEntityTypeConfiguration<RestaurantCuisine>
    {

        public void Configure(EntityTypeBuilder<RestaurantCuisine>builder)
        {
            builder.HasKey(rc => rc.Id);

            builder.HasIndex(rc => new {rc.RestaurantId, rc.CuisineId})
                .IsUnique();

            builder.HasQueryFilter(rc => !rc.IsDeleted);

            builder.HasOne(rc => rc.Restaurant)
                .WithMany(r => r.RestaurantCuisines)
                .HasForeignKey(rc => rc.RestaurantId)
                .OnDelete(DeleteBehavior.Cascade);
            
            builder.HasOne(rc => rc.Cuisine)
                .WithMany(c => c.RestaurantCuisines)
                .HasForeignKey(rc => rc.CuisineId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}