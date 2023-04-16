using Shop.Api.Data;

namespace Shop.Api.Models.Order
{
    public class AllUserDTO
    {
      public IList<UserApp>? users { get;set; }
      public int totals { get; set; } 
    }
}
