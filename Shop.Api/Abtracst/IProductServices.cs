using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Products;
using Shop.Api.Data;
using Shop.Api.Models;

namespace Shop.Api.Abtracst
{
    public interface IProductServices
    {
        public Task<PageProduct> GetProductAsync(SearchModel search);
        public Task<ProductDTO> GetOneProductAsync(Guid id);
        public Task<string> AddProductAsysnc(ProductAdd product);
        public Task<string> AddSizeProductAsync(AddSize<StringSize> stringSizes);
        public Task<string> AddProductAzureAsync(ProductAdd product);
        public Task<string> GetProductName(Guid id);
        public Task<bool> UpdateQuantySizeAsync(int quanlity, Guid id, string size);
        public Task<int> CheckQtyAsync(int quanlity, Guid id, string size);
        public Task<bool> SetProductAsync( ProductAdd product);
    }
}
