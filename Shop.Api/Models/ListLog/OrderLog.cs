using Shop.Api.Abtracst;

namespace Shop.Api.Models.ListLog
{
    public class OrderLog : ResponeModel<string, bool>
    {
        public Guid Id { get; set; }
        public string UrlPayment { get; set; }
        public override string Log(string message)
        {
            return message;
        }
        public OrderLog()
        {
            Status = true;
        } 
    }
}
