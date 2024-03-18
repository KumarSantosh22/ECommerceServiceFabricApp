using ECommerce.API.Models;
using ECommerce.Domain;
using ECommerce.Domain.Contracts.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Services.Client;
using Microsoft.ServiceFabric.Services.Remoting.Client;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Client;

namespace ECommerce.API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ProductsController : ControllerBase
    {
        private readonly IProductCatalogService _productCatalogService;

        public ProductsController()
        {
            ServiceProxyFactory proxyFactory = new ServiceProxyFactory(context => new FabricTransportServiceRemotingClientFactory());

            _productCatalogService = proxyFactory.CreateServiceProxy<IProductCatalogService>(
                new Uri("fabric:/ECommerce/ECommerce.ProductCatalog"), new ServicePartitionKey(0));
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<ProductDto>>> Get()
        {
            IEnumerable<Product> products =  await _productCatalogService.GetAllProductsAsync();

            return Ok(
                products.Select(p => new ProductDto
                {
                    Id = p.Id,
                    Name = p.Name,
                    Description = p.Description,
                    IsAvailable = p.Availability > 0,
                    Price = p.Price
                })
            );
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] ProductDto dto)
        {
            Product product = new Product
            {
                Id = dto.Id,
                Name = dto.Name,
                Description = dto.Description,
                Price = dto.Price,
                Availability = 100
            };
            await _productCatalogService.AddProductAsync(product);
            return Created();
        }
    }
}
