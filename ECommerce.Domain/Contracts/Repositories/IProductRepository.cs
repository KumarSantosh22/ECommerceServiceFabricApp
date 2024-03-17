namespace ECommerce.Domain.Contracts.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();

        Task<Product> Add(Product product);

        Task<Product> Get(Guid productId);

    }
}
