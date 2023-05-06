namespace Shop.Api.Models.Order
{
    public class AcceptPayRequest
    {
        public Guid Id { get; set; }
        public string? Status { get; set; } 
    }
}
