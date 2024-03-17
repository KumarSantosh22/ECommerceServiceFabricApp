using ECommerce.Domain;

namespace ECommerce.API.Models
{
    public class CheckoutSummaryDto
    {
        public List<CheckoutProductDto> Products { get; set; }

        public double TotalPrice { get; set; }

        public DateTime Date { get; set; }
    }
}
