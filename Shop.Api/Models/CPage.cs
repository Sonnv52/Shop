namespace Shop.Api.Models
{
    public class CPage<T>: List<T>
    {
        public int PageIndex { get; set; }
        public int TotalPage { get; set; }
        public CPage(List<T> items, int count) { 
        
        }
    }
}
