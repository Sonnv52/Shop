namespace Shop.Api.Models
{
    public class SearchModel
    {
        public string? key { get; set; }
        public double? from { get; set; }
        public double? to { get; set; }
        public string ? sort { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
    }
}
