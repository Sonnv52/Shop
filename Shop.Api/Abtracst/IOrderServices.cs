using Shop.Api.Data;
using Shop.Api.Models;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;
using Shop.Api.Models.Page;

namespace Shop.Api.Abtracst
{
    public interface IOrderServices
    {
        Task<OrderLog> OrderAsync(OrderRequest request, string email, CancellationToken token);
        Task<double> GetPriceAsync(IList<ProductsRequest?> products);
        Task<IList<BillDTO?>> GetBillsAsync(string email);
        Task<bool> SetBillAsync(IList<SetBillRequest> setBills);
        Task<PagedList<BillAdminDTO>> GetAllBillAsync(PageQuery page);
        Task<BillDetailDTO> GetBillDetailAsync(Guid id);
        Task<IList<BillList>> GetYourBillAsync(string email);
        Task<BillDetailDTO> GetYourBillDetaillAsync(string email, Guid idBill);
        Task<string> CancelAsync(string email, Guid id);
        Task<bool> AcceptPayAsync(Guid id, string mail, string status);
    }
}
