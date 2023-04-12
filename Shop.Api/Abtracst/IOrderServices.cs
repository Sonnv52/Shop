using Shop.Api.Models;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;

namespace Shop.Api.Abtracst
{
    public interface IOrderServices
    {
        public Task<OrderLog> OrderAsync(OrderRequest request, string email, CancellationToken token);
        public Task<double> GetPriceAsync(IList<ProductsRequest?> products);
        public Task<IList<BillDTO?>> GetBillsAsync(string email);
        Task<bool> SetBillAsync(IList<SetBillRequest> setBills);
        Task<IList<BillAdminDTO>> GetAllBillAsync();
    }
}
