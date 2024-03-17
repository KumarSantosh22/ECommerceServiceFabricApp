using System.Fabric;
using ECommerce.Domain;
using ECommerce.Domain.Contracts.Repositories;
using ECommerce.Domain.Contracts.Services;
using ECommerce.ProductCatalog.Providers.Repositories;
using Microsoft.ServiceFabric.Services.Communication.Runtime;
using Microsoft.ServiceFabric.Services.Remoting.V2.FabricTransport.Runtime;
using Microsoft.ServiceFabric.Services.Runtime;

namespace ECommerce.ProductCatalog
{
    /// <summary>
    /// An instance of this class is created for each service replica by the Service Fabric runtime.
    /// </summary>
    public sealed class ProductCatalog : StatefulService, IProductCatalogService
    {
        private IProductRepository _repository;

        public ProductCatalog(StatefulServiceContext context)
            : base(context)
        { }

        public async Task AddProductAsync(Product product)
        {
            await _repository.Add(product);
        }

        public async Task<Product[]> GetAllProductsAsync()
        {
            return (await _repository.GetAll()).ToArray();
        }

        public async Task<Product> GetProductAsync(Guid productId)
        {
            return await _repository.Get(productId);
        }

        /// <summary>
        /// Optional override to create listeners (e.g., HTTP, Service Remoting, WCF, etc.) for this service replica to handle client or user requests.
        /// </summary>
        /// <remarks>
        /// For more information on service communication, see https://aka.ms/servicefabricservicecommunication
        /// </remarks>
        /// <returns>A collection of listeners.</returns>
        protected override IEnumerable<ServiceReplicaListener> CreateServiceReplicaListeners()
        {
            return
            [
                new  ServiceReplicaListener(context => new FabricTransportServiceRemotingListener(context, this))
            ];
        }

        /// <summary>
        /// This is the main entry point for your service replica.
        /// This method executes when this replica of your service becomes primary and has write status.
        /// </summary>
        /// <param name="cancellationToken">Canceled when Service Fabric needs to shut down this service replica.</param>
        protected override async Task RunAsync(CancellationToken cancellationToken)
        {
            _repository = new ProductRepository(this.StateManager);

            Product product1 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Dell Monitor",
                Description = "Computer Monitor",
                Price = 500,
                Availability = 100
            };
            Product product2 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Surface Book",
                Description = "Microsoft's Latest Laptop, i7 CPU, 1Tb SSD",
                Price = 2200,
                Availability = 15
            };
            Product product3 = new()
            {
                Id = Guid.NewGuid(),
                Name = "Arc Touch Mouse",
                Description = "Computer Mouse, bluetooth, requires 2 AAA batteries",
                Price = 60,
                Availability = 30
            };

            _ = await _repository.Add(product1);
            _ = await _repository.Add(product2);
            _ = await _repository.Add(product3);

            IEnumerable<Product> products = await _repository.GetAll();

        }
    }
}
