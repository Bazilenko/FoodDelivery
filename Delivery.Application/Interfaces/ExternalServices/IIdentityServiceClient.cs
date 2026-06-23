
namespace Delivery.Application.Interfaces.ExternalServices
{
    public interface IIdentityServiceClient
    {
        Task AssignCourierRoleAsync(string userId, CancellationToken ct = default);
    }
}