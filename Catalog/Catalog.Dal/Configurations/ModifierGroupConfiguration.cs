using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class ModifierGroupConfiguration : IEntityTypeConfiguration<ModifierGroup>
    {
        public void Configure(EntityTypeBuilder<ModifierGroup> builder)
        {
            builder.HasKey(m => m.Id);

            builder.Property(m => m.Name)
            .IsRequired()
            .HasMaxLength(100);

            builder.Property(m => m.MinSelect)
            .IsRequired();
 
            builder.Property(m => m.MaxSelect)
            .IsRequired();

            builder.HasQueryFilter(mg => !mg.IsDeleted);

            builder.HasOne(m => m.Dish)
                .WithMany(d => d.ModifierGroups)
                .HasForeignKey(m => m.DishId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.HasMany(m => m.DishOptions)
                .WithOne(d => d.ModifierGroup)
                .HasForeignKey(d => d.ModifierGroup)
                .OnDelete(DeleteBehavior.Cascade);




        }
    }
}