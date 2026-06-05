
namespace Delivery.Infrastructure.ExternalServices
{
    public interface IIdentityServiceClient
    {
        Task AssignCourierRoleAsync(string userId, CancellationToken ct = default);
    }
}