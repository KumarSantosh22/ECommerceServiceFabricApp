using Microsoft.ServiceFabric.Services.Remoting;

namespace ECommerce.Domain.Contracts.Services
{
    public interface ICheckoutService: IService
    {
        Task<CheckoutSummary> CheckoutAsync(string userId);

        Task<CheckoutSummary[]> GetOrderHitoryAsync(string userId);
    }
}
