using Azure.Storage.Blobs;
using Shop.Api.Abtracst;
using Shop.Api.Models.ListLog;
using Shop.Api.Data;

namespace Shop.Api.Repository
{
    public class ImageResponsitory : IImageServices
    {
        private readonly IConfiguration _configuration;
        public ImageResponsitory(IConfiguration configuration)
        {
            _configuration = configuration;
        }

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

        public byte[]? Parse(string url)
        {
            var imagePath = Path.Combine(url);

            if (System.IO.File.Exists(imagePath))
            {
                return  System.IO.File.ReadAllBytes(imagePath);
            }

            return null;
        } 
        public async Task<string> PostImageToAzureAsync(IFormFile file)
        {

            BlobContainerClient blod = new BlobContainerClient(_configuration["AzureString"], "shoimage");
            var FileName = $"{file.FileName}-{Guid.NewGuid()}";
            using (var stream = new MemoryStream())
            {
                await file.CopyToAsync(stream);
                stream.Position = 0;
                await blod.UploadBlobAsync(FileName, stream);
            }
            return $"{_configuration["LinkAzure"]}-{FileName}";
        }

        public Task<ImageLog> RemoveAsync()
        {
            throw new NotImplementedException();
        }
    }
}
