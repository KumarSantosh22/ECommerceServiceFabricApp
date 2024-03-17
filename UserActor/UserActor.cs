using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Runtime;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace UserActor
{
    /// <remarks>
    /// This class represents an actor.
    /// Every ActorID maps to an instance of this class.
    /// The StatePersistence attribute determines persistence and replication of actor state:
    ///  - Persisted: State is written to disk and replicated.
    ///  - Volatile: State is kept in memory only and replicated.
    ///  - None: State is kept in memory only and not replicated.
    /// </remarks>
    [StatePersistence(StatePersistence.Persisted)]
    internal class UserActor : Actor, IUserActor
    {
        public UserActor(ActorService actorService, ActorId actorId)
            : base(actorService, actorId)
        {
        }

        public async Task AddToBasket(Guid productId, int quantity)
        {
            await StateManager.AddOrUpdateStateAsync(productId.ToString(), quantity, (Id, oldQty) => oldQty + quantity);
        }

        public async Task ClearBasket()
        {
            IEnumerable<string> productIds = await StateManager.GetStateNamesAsync();
            foreach (string productId in productIds)
            {
                await StateManager.RemoveStateAsync(productId);
            }
        }

        public async Task<BasketItem[]> GetBasket()
        {
            List<BasketItem> result = new List<BasketItem>();

            IEnumerable<string> productIds = await StateManager.GetStateNamesAsync();
            foreach (string productId in productIds)
            {
                int quantity = await StateManager.GetStateAsync<int>(productId);
                result.Add(new BasketItem { ProductId = new Guid(productId), Quantity = quantity });
            }

            return result.ToArray();
        }

    }
}
