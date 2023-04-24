namespace Shop.Api.Models
{
    public class PageProduct
    {
        public IList<ProductOnlyDTO>? Products { get; set; }
        public int TotalPages { get; set; } 
    }
}
