namespace Shop.Api.Models.Order
{
    public class ProductsRequest
    {
        public Guid id { get; set; }
        public string? Size { get; set; }
        public int Qty { get; set; }
        public double? Price { get; set; }
    }
}
