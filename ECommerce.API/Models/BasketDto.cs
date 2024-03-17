namespace ECommerce.API.Models
{
    public class BasketDto
    {
        public string UserId { get; set; }

        public BasketItemDto[] Items { get; set; }        
    }

    public class BasketItemDto
    {
        public string ProductId { get; set; }

        public int Quantity { get; set; }
    }
}
