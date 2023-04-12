namespace Shop.Api.Models.Order
{
    public class BillAdminDTO
    {
        public Guid Id { get; set; }
        public DateTime OderDate { get; set; }
        public string? Adress { get; set; }
        public string Phone { get; set; } = string.Empty;
        public string? Status { get; set; }
        public Guid IdUser { get; set; }    
        public string? Name { get; set; }
    }
}
