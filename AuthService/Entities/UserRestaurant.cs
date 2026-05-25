
namespace Auth.Entities
{
    public class UserRestaurant
    {
        public int UserId { get; set; }
        public int RestaurantId { get; set; }
        public ApplicationUser User { get; set; } = null!;
    }
}