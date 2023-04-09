namespace Shop.Api.Models
{
    public class ListBillsDTO
    {
        public IList<BillDTO>? BillList { get; set; }
        public string? BillName { get; set; }
    }
}
