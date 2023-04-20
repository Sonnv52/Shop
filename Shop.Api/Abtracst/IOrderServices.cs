using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;
using Shop.Api.Models.Page;

namespace Shop.Api.Abtracst
{
    public interface IOrderServices
    {
        public Task<OrderLog> OrderAsync(OrderRequest request, string email, CancellationToken token);
        public Task<double> GetPriceAsync(IList<ProductsRequest?> products);
        public Task<IList<BillDTO?>> GetBillsAsync(string email);
        Task<bool> SetBillAsync(IList<SetBillRequest> setBills);
        Task<PagedList<BillAdminDTO>> GetAllBillAsync(int page, int pageSize);
        Task<BillDetailDTO> GetBillDetailAsync(Guid id);
        Task<IList<BillList>> GetYourBillAsync(string email);
        Task<BillDetailDTO> GetYourBillDetaillAsync(string email, Guid idBill);
    }
}
