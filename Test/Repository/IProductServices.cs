using Test.Data;
using Test.Models;

namespace Test.Repository
{
    public interface IProductServices
    {
        public Task<IEnumerable<ProductT>> GetProductAsync(SearchModel search);
    }
}
