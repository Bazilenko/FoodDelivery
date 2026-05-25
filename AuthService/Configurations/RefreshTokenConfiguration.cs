using Microsoft.EntityFrameworkCore;
using Auth.Entities;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
namespace Auth.Configurations;
public class RefreshTokenConfiguration : IEntityTypeConfiguration<RefreshToken>
{
    public void Configure(EntityTypeBuilder<RefreshToken> builder)
    {
        builder.HasKey(rt => rt.Id);
 
        builder.Property(rt => rt.Token)
            .IsRequired()
            .HasMaxLength(500);
 
        builder.Property(rt => rt.ReplacedByToken)
            .HasMaxLength(500);
 
        builder.Property(rt => rt.RevokedReason)
            .HasMaxLength(200);
 
        // Fast lookup by token value
        builder.HasIndex(rt => rt.Token).IsUnique();
 
        // Fast lookup of all tokens for a user
        builder.HasIndex(rt => rt.UserId);
 
        builder.HasOne(rt => rt.User)
            .WithMany(u => u.RefreshTokens)
            .HasForeignKey(rt => rt.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}