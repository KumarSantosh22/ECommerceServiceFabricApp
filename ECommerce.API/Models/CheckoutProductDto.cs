namespace ECommerce.API.Models
{
    public class CheckoutProductDto
    {
        public Guid Id { get; set; }

        public required string Name { get; set; }

        public required string Description { get; set; }

        public double Price { get; set; }

        public int Quantity { get; set; }

    }
}
