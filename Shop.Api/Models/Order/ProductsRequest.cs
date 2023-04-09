using Microsoft.Build.Framework;

namespace Shop.Api.Models.Order
{
    public class ProductsRequest
    {
        [Required]
        public Guid id { get; set; }
        [Required]
        public string? Size { get; set; }
        [Required]
        public int Qty { get; set; }
        public double? Price { get; set; }
    }
}
