namespace ECommerce.Domain.Contracts.Repositories
{
    public interface IProductRepository
    {
        Task<IEnumerable<Product>> GetAll();

        //Task<Product> Get(int id);

        Task<Product> Add(Product product);

        //Task<Product> Update(Product product);

        //Task Delete(int id);

    }
}
