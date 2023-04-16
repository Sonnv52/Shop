namespace Shop.Api.Models.Order
{
    public class BillDetailDTO
    {
        public Guid GuidId { get; set; }
        public DateTime OrderDate { get; set; }
        public string? OrderStatus { get; set; }
        public string? Email { get; set;}
        public IList<DetailBill?>? Detail  { get; set; }
    }
    public class DetailBill
    {
        public Guid ProductId { get; set;}
        public string? ProductName { get; set;}
        public double? Price { get; set; } = 0;
        public int Qty { get; set; } = 0;
        public byte[]? IM { get; set; }
    }
}
