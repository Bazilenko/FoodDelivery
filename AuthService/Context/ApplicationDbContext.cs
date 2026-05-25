using Auth.Entities;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;


namespace Auth.Context;
public class ApplicationDbContext : IdentityDbContext<ApplicationUser, IdentityRole<int>, int>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<UserRestaurant> UserRestaurant => Set<UserRestaurant>();
 
    public DbSet<RefreshToken> RefreshTokens => Set<RefreshToken>();
 
    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        builder.Entity<UserRestaurant>().ToTable("UserRestaurant");
        
        builder.Entity<UserRestaurant>()
        .HasKey(ur => new { ur.UserId, ur.RestaurantId });

        builder.Entity<UserRestaurant>()
        .HasOne(ur => ur.User)
        .WithMany(u => u.UserRestaurants)
        .HasForeignKey(ur => ur.UserId); 
        builder.ApplyConfigurationsFromAssembly(typeof(ApplicationDbContext).Assembly);
    }
}
