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
using Shop.Api.Abtracst;
using Shop.Api.Models;
using Microsoft.CodeAnalysis;
using Shop.Api.Repository;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace Test.Repository
{
    public class ProductRepository : IProductServices
    {
        private readonly NewDBContext _dbContext;
        private readonly IMapper _mapper;
        private readonly IImageServices _imageServices;
        public ProductRepository(IImageServices imageServices, NewDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
            _imageServices = imageServices;
        }

        public async Task<ProductDTO> GetOneProductAsync(Guid id)
        {

            var product = _dbContext.Products.Include(p => p.Sizes).FirstOrDefault(p => p.Id == id);
            var size = product?.Sizes.Select(s => _mapper.Map<SizeDTO>(s)).ToList();
            if (product == null)
            {
                throw new Exception("product not found!!!");
            }
            return new ProductDTO
            {
                Id = product.Id,
                Name = product.Name,
                Description = product.Description,
                Price = product.Price,
                Image = product.Image,
                IM = await _imageServices.ParseAsync(product.Image),
                Sizes = size
            };
        }

        public async Task<PageProduct> GetProductAsync(SearchModel search)
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
                products = products.Where(pr => pr.Name.Contains(search.key));

            }

            var totalItems = products.Count();
            var totalPages = (int)Math.Ceiling((double)totalItems / search.PageSize);

            var pagedProducts = products.Skip(search.PageSize * (search.PageIndex - 1))
                                         .Take(search.PageSize)
                                         .Select(pro => _mapper.Map<ProductT>(pro))
                                         .ToList();

            var imagePaths = pagedProducts.Select(p => Path.Combine(p.Image ?? "")).ToList();
            var imageDataList = await Task.WhenAll(imagePaths.Select(p => System.IO.File.ReadAllBytesAsync(p)));

            for (int i = 0; i < pagedProducts.Count; i++)
            {
                pagedProducts[i].IM = imageDataList[i];
            }

            return new PageProduct
            {
                Products = pagedProducts,
                TotalPages = totalPages
            };
        }

        public async Task<string> AddProductAsysnc(ProductAdd product)
        {
            if (product.Image != null)
            {
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "image", "products", imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }

#pragma warning disable CS8629 // Nullable value type may be null.
                var pro = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = product.Name != null ? product.Name : "Unknow",
                    Price = (double)product.Price ,
                    Description = product.Description != null ? product.Description : "Unknow",
                    Image = imagePath,
                };
#pragma warning restore CS8629 // Nullable value type may be null.

                var i = await _dbContext.Products.AddAsync(pro);
                await _dbContext.SaveChangesAsync();
                return pro.Id.ToString();
            }
            return "Image or category maybe null!!";
        }

        public async Task<string> AddSizeProductAsync(AddSize<StringSize> stringSizes)
        {
            Product? product = await _dbContext.Products.FirstOrDefaultAsync(pro => pro.Id == stringSizes.ProductID);
            if (product == null)
            {
                throw new Exception("No exs product");
            }
            if(stringSizes.StringSize == null) {
                return "Size cant be null!!";
                    }
            foreach (var i in stringSizes.StringSize)
            {
                var Size = new Size
                {
                    IdSizelog = Guid.NewGuid(),
                    size = i.SizeProduct,
                    Qty = i.Qty,
                    Products = product
                };
                try
                {
                    await _dbContext.Sizes.AddAsync(Size);
                }
                catch (Exception)
                {
                    return $"false to add for {product.Name}";
                }
            }
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
            return "Suggest!!";
        }

        public Task<string> AddProductAzureAsync(ProductAdd product)
        {
            throw new NotImplementedException();
        }

        public async Task<string> GetProductName(Guid id)
        {
            Product? product = await _dbContext.Products.FindAsync(id);
            string? name = product?.Name;
            return name ?? string.Empty;
        }

        public Task<bool> UpdateQuantySizeAsync(int quanlity, Guid id, string size)
        {
            var productSize = _dbContext.Sizes.Where(s => s.Products.Id == id && s.size == size);
            var szUpdate = productSize.FirstOrDefault();

            if (szUpdate == null)
            {
                throw new Exception("Size not found.");
            }

            if (szUpdate.Qty < quanlity)
            {
                return Task.FromResult(false);
            }

            szUpdate.Qty -= quanlity;
            _dbContext.Entry(szUpdate).State = EntityState.Modified;
            _dbContext.SaveChanges();
            return Task.FromResult(true);
        }

        public async Task<int> CheckQtyAsync(int quanlity, Guid id, string size)
        {
            var productSize = _dbContext.Sizes.Where(s => s.Products.Id == id && s.size == size);
            var szUpdate = await productSize.FirstOrDefaultAsync();
            if(szUpdate == null)
            {
                return 0;
            }
            return  szUpdate.Qty - quanlity;
        }
    }
}
