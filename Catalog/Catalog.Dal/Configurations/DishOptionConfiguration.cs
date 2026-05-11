using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Catalog.Dal.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Dal.Configurations
{
    public class DishOptionConfiguration : IEntityTypeConfiguration<DishOption>
    {
        public void Configure(EntityTypeBuilder<DishOption> builder)
        {
            builder.HasKey(d => d.Id);

            builder.Property(d => d.Name)
                .IsRequired()
                .HasMaxLength(100);

            builder.Property(d => d.Price)
                .IsRequired()
                .HasColumnType("decimal(10,2)");

            builder.HasQueryFilter(o => !o.IsDeleted);

            builder.HasOne(d => d.ModifierGroup)
                .WithMany(m => m.DishOptions)
                .HasForeignKey(d => d.ModifierGroupId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
