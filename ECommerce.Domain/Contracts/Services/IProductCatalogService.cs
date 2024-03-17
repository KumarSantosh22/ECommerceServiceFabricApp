using Microsoft.ServiceFabric.Services.Remoting;

namespace ECommerce.Domain.Contracts.Services
{
    public interface IProductCatalogService: IService
    {
        Task<Product[]> GetAllProductsAsync();

        Task AddProductAsync(Product product);

        Task<Product> GetProductAsync(Guid productId);
    }
}
