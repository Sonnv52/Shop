namespace Shop.Api.Models.Order
{
    public class PayModel
    {
        public string? Amount { get; set; }
        public string? PaymentMthods { get; set; }
        public Guid OrderID { get; set; }
    }
}