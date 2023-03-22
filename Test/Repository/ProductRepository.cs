using AutoMapper;
using System.Drawing.Printing;
using Test.Data;
using Test.Models;
using X.PagedList;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Shop.Api.Models.CreateModel;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Enums;
using Shop.Api.Data;
using Shop.Api.Models.Products;

namespace Test.Repository
{
    public class ProductRepository: IProductServices
    {
        private readonly NewDBContext _dbContext;
        private readonly IMapper _mapper;
        public static IWebHostEnvironment _environment;
        public ProductRepository(NewDBContext dbContext, IMapper mapper, IWebHostEnvironment environment)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _environment = environment;
        }

        public Task<Product> GetOneProductAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<PageProduct> GetProductAsync(SearchModel search, PagingSearch paging)
        {
            var products = _dbContext.Products.AsQueryable();
            #region sort
            if (!String.IsNullOrEmpty(search.sort))
            {
                switch (search.sort)
                {
                    case "Namekey": products = products.OrderBy(x => x.Name); break;
                    case "Price_up": products = products.OrderBy(x => x.Price); break;
                    case "Price_down": products = products.OrderByDescending(x => x.Price); break;
                }
            }
            #endregion
            #region Fillter

            if (search.from != null)
            {
                products = products.Where(pro => pro.Price >= search.from);
            }
            if (search.to != null)
            {
                products = products.Where(pro => pro.Price <= search.to);
            }
            #endregion
            if (!String.IsNullOrEmpty(search.key))
            {
                products = products.Where(pr => pr.Name.Contains(search.key) || pr.Type.Name.Contains(search.key) ||
                pr.Type.Id.ToString().Equals(search.key));

            }

            IEnumerable<ProductT> returns = products.Select(pro => _mapper.Map<ProductT>(pro));
            var total = returns.Count();
            returns = returns.ToPagedList(paging.PageIndex, paging.PageSize);
            foreach (var item in returns)
            {
                var imagePath = Path.Combine(item.Image);

                if (System.IO.File.Exists(imagePath))
                {
                    item.IM = await System.IO.File.ReadAllBytesAsync(imagePath);
                }
            }
            return new PageProduct
            {

                Products = returns.ToList(),
                totalPage = total
            };
        }

        public async Task<string> AddProductAsysnc(ProductAdd product)
        {
            if(product.Image != null && product.type != 0)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "image", "products", imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }

                var pro = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = product.Name != null ? product.Name : "Unknow",
                    Price = (double)product.Price,
                    Description = product.Description != null ? product.Description : "Unknow",
                    Image = imagePath,
                };
  
                var type = await _dbContext.Types.FirstOrDefaultAsync(ty => ty.Id == product.type);
         
                if (type == null)
                {
                    return "false for category!!";
                }

                pro.Type = type;
                var i = await _dbContext.Products.AddAsync(pro);
                await _dbContext.SaveChangesAsync();
                return pro.Id.ToString();
            }
            return "Image or category maybe null!!";
        }

        public async Task<string> AddSizeProductAsync(AddSize<StringSize> stringSizes)
        {
            Product product = await _dbContext.Products.FirstOrDefaultAsync(pro => pro.Id == stringSizes.ProductID);
            if (product == null)
            {
                return "Product not found!!";
            }
            foreach(var i in stringSizes.StringSize)
            {
                var Size = new Size
                {
                    IdSize = Guid.NewGuid(),
                    size = i.SizeProduct,
                    Qty = i.Qty
                };
                try
                {
                    await _dbContext.Sizes.AddAsync(Size);
                }catch(Exception ex)
                {
                    return $"false to add for {product.Name}";
                }
            }
             await _dbContext.SaveChangesAsync();
            return "Suggest!!";
        }


        /* private async Task<string> SaveFile(IFormFile file)
{
    var originalFileName = ContentDispositionHeaderValue.Parse(file.ContentDisposition).FileName.TrimEnd('"');
    var fileName = $"{Guid.NewGuid()}{Path.GetExtension(originalFileName)}";
    await _storageService.SaveFileAsync(file.OpenReadStream(), fileName);
    return "/" + USER_CONTENT_FOLDER_NAME + "/" + fileName;

}*/
    }
}
