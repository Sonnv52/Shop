namespace Shop.Api.Models
{
    public class PageProduct
    {
        public IList<ProductT>? Products { get; set; }
        public int TotalPages { get; set; } 
    }
}
