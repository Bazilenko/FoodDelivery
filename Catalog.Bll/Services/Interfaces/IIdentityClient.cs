
namespace Catalog.Bll.Services.Interfaces
{
    public interface IIdentityClient
    {
     Task LinkUserToRestaurantAsync(int userId, int restaurantId, CancellationToken ct = default);
    }
}