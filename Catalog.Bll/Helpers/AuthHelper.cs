using Microsoft.AspNetCore.Http;
using System.Security.Claims;

namespace Catalog.Bll.Helpers
{
    public static class AuthHelper
    {
        public static int GetRestaurantIdFromToken(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var restaurantIdClaim = httpContext?.User?.FindFirst("restaurant_id")?.Value;

            if (string.IsNullOrEmpty(restaurantIdClaim))
            {
                throw new UnauthorizedAccessException("У вашому JWT токені відсутній ідентифікатор ресторану (restaurant_id).");
            }

            return int.Parse(restaurantIdClaim);
        }

        public static int GetUserIdFromToken(IHttpContextAccessor httpContextAccessor)
        {
            var httpContext = httpContextAccessor.HttpContext;
            var userIdClaim = httpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;

            if (string.IsNullOrEmpty(userIdClaim))
            {
                throw new UnauthorizedAccessException("У вашому JWT токені відсутній ідентифікатор користувача (UserId).");
            }

            return int.Parse(userIdClaim);
        }
    }
}