using Shop.Api.Abtracst;
using Shop.Api.Models.ListLog;

namespace Shop.Api.Repository
{
    public class ImageResponsitory : IImageServices
    {
        public Task<ImageLog> AddAsync()
        {
            throw new NotImplementedException();
        }

        public async Task<byte[]?> ParseAsync(string url)
        {
            var imagePath = Path.Combine(url);

            if (System.IO.File.Exists(imagePath))
            {
                return await System.IO.File.ReadAllBytesAsync(imagePath);
            }
            return null;
        }

        public Task<ImageLog> RemoveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
