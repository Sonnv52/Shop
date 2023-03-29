using Shop.Api.Models;
using Shop.Api.Models.CreateModel;
using Shop.Api.Models.Products;
using Test.Data;
using Test.Models;

namespace Shop.Api.Abtracst
{
    public interface IProductServices
    {
        public Task<PageProduct> GetProductAsync(SearchModel search);
        public Task<ProductDTO> GetOneProductAsync(Guid id);
        public Task<string> AddProductAsysnc(ProductAdd product);
        public Task<string> AddSizeProductAsync(AddSize<StringSize> stringSizes);
        public Task<string> AddProductAzureAsync(ProductAdd product);
    }
}
