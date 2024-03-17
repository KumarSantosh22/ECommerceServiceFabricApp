using ECommerce.Domain;
using ECommerce.Domain.Contracts.Repositories;
using Microsoft.ServiceFabric.Data;
using Microsoft.ServiceFabric.Data.Collections;

namespace ECommerce.ProductCatalog.Providers.Repositories
{
    public class ProductRepository : IProductRepository
    {
        private readonly IReliableStateManager _stateManager;
        public ProductRepository(
            IReliableStateManager stateManager)
        {
            _stateManager = stateManager;
        }

        public async Task<Product> Add(Product product)
        {
            Product response;

            IReliableDictionary<Guid, Product> products = await _stateManager
            .GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                response = await products
                    .AddOrUpdateAsync(tx, product.Id, product, (id, value) => product);

                await tx.CommitAsync();
            }
            return response;
        }

        public async Task<IEnumerable<Product>> GetAll()
        {
            IReliableDictionary<Guid, Product> products =
            await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");
            List<Product> result = new List<Product>();

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                Microsoft.ServiceFabric.Data.IAsyncEnumerable<KeyValuePair<Guid, Product>> allProducts =
                   await products.CreateEnumerableAsync(tx, EnumerationMode.Unordered);

                using (Microsoft.ServiceFabric.Data.IAsyncEnumerator<KeyValuePair<Guid, Product>> enumerator =
                   allProducts.GetAsyncEnumerator())
                {
                    while (await enumerator.MoveNextAsync(CancellationToken.None))
                    {
                        KeyValuePair<Guid, Product> current = enumerator.Current;
                        result.Add(current.Value);
                    }
                }
            }

            return result;
        }

        public async Task<Product> Get(Guid productId)
        {
            IReliableDictionary<Guid, Product> products = await _stateManager.GetOrAddAsync<IReliableDictionary<Guid, Product>>("products");

            using (ITransaction tx = _stateManager.CreateTransaction())
            {
                ConditionalValue<Product> product = await products.TryGetValueAsync(tx, productId);

                return product.HasValue ? product.Value : null;
            }
        }
    }
}
