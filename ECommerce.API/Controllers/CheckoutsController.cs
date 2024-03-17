using ECommerce.API.Models;
using ECommerce.Domain;
using ECommerce.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class CheckoutsController : ControllerBase
    {
        private static readonly Random random = new Random(DateTime.UtcNow.Second);

        [HttpGet("{userId}")]
        public async Task<ActionResult<CheckoutSummaryDto>> CheckoutAsync(string userId)
        {
            CheckoutSummary summary = await GetCheckoutService().CheckoutAsync(userId);
            CheckoutSummaryDto response = new CheckoutSummaryDto()
            {
                TotalPrice = summary.TotalPrice,
                Date = summary.Date,
                Products = summary.Products.Select(p => new CheckoutProductDto()
                {
                    Name = p.Product.Name,
                    Description = p.Product.Description,
                    Id = p.Product.Id,
                    Quantity = p.Quantity
                }).ToList()
            };

            return Ok(response);
        }

        [HttpGet("history/{userId}")]
        public async Task<ActionResult<CheckoutSummaryDto>> GetHistoryAsync(string userId)
        {
            CheckoutSummary[] summary = await GetCheckoutService().GetOrderHitoryAsync(userId);
            CheckoutSummaryDto[] response = summary.Select(p => new CheckoutSummaryDto()
            {

            }).ToArray();

            return Ok(response);
        }

        private ICheckoutService GetCheckoutService()
        {
            long key = LongRandom();
            ServiceProxyFactory proxyFactory = new ServiceProxyFactory(c => new FabricTransportServiceRemotingClientFactory());

            return proxyFactory.CreateServiceProxy<ICheckoutService>(
                new Uri("fabric:/ECommerce/ECommerce.CheckoutService"),
                new ServicePartitionKey(key));
        }

        private long LongRandom()
        {
            byte[] bytes = new byte[8];
            random.NextBytes(bytes);
            return BitConverter.ToInt64(bytes, 0);
        }
    }
}
