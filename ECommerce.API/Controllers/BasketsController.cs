using ECommerce.API.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ServiceFabric.Actors;
using Microsoft.ServiceFabric.Actors.Client;
using UserActor.Interfaces;

namespace ECommerce.API.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class BasketsController : ControllerBase
    {
        [HttpGet("{userId}")]
        public async Task<ActionResult<BasketDto>> GetAsync(string userId)
        {
            IUserActor actor = GetActor(userId);
            BasketItem[] products = await actor.GetBasket();

            BasketDto response = new()
            {
                UserId = userId,
                Items = products.Select(
                    p => new BasketItemDto { ProductId = p.ProductId.ToString(), Quantity = p.Quantity }
                    ).ToArray()
            };

            return Ok(response);
        }

        [HttpPost("{userId}")]
        public async Task<ActionResult> PostAsync(string userId, [FromBody] BasketItemDto basketItem)
        {
            IUserActor actor = GetActor(userId);
            await actor.AddToBasket(new Guid(basketItem.ProductId), basketItem.Quantity);
            return Created();
        }

        [HttpDelete("{userId}")]
        public async Task<ActionResult> DeleteAsync(string userId)
        {
            IUserActor actor = GetActor(userId);
            await actor.ClearBasket();
            return Ok("Basket is empty now!");
        }

        private IUserActor GetActor(string userId)
        {
            return ActorProxy.Create<IUserActor>(
                new ActorId(userId),
                new Uri("fabric:/ECommerce/UserActorService"));
        }
    }
}
