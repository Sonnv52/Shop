using Shop.Api.Models.ListLog;

namespace Shop.Api.Abtracst
{
    public interface IImageServices
    {
        public Task<ImageLog> AddAsync();
        public Task<ImageLog> RemoveAsync();
        public Task<byte[]?> ParseAsync(string url);
        public Task<string> PostImageToAzureAsync(IFormFile file);
    }
}
