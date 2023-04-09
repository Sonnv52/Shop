namespace Shop.Api.Models
{
    public class BillDTO
    {
        public DateTime OderDate { get; set; }
        public double? Price { get; set; } = 0;
        public string? Size { get; set; }
        public string? Name { get; set; }
        public string? Image { get; set; }
        public int? Total { get; set; }
        public byte[]? IM { get; set; }
    }
}
