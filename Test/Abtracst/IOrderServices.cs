using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;

namespace Shop.Api.Abtracst
{
    public interface IOrderServices<T>
    {
        public Task<OrderLog> OrderAsync(OrderRequest request, string email);
        public Task<double> GetPriceAsync(IList<ProductsRequest?> products);
    }
}
