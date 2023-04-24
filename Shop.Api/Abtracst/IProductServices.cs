using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Products;
using Shop.Api.Data;

namespace Shop.Api.Abtracst
{
    public interface IProductServices
    {
        public Task<PageProduct> GetProductAsync(SearchModel search);
        public Task<ProductDTO> GetOneProductAsync(Guid id);
        public Task<string> AddProductAsysnc(ProductAddModel product);
        public Task<string> AddSizeProductAsync(AddSizeModel<StringSize> stringSizes);
        public Task<string> AddProductAzureAsync(ProductAddModel product);
        public Task<string> GetProductName(Guid id);
        public Task<bool> UpdateQuantySizeAsync(int quanlity, Guid id, string size);
        public Task<int> CheckQtyAsync(int quanlity, Guid id, string size);
        public Task<bool> SetProductAsync( ProductAddModel product);
        public Task<bool> Set2ProductAsync(ProductAddModel product);
    }
}
