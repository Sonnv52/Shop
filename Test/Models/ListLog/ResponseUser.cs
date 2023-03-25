using Shop.Api.Abtracst;

namespace Shop.Api.Models.ListLog
{
    public class ResponseUser : ResponeModel<string>
    {
        public override string Log(string message)
        {
           return message;
        }
    }
}
