using Shop.Api.Data;
using Shop.Api.Models.Order;

namespace Shop.Api.Abtracst
{
    public interface IPayService
    {
        public Task<string> GetUrlPayAsync(PayModel pay);
    }
}