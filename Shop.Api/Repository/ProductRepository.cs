﻿using AutoMapper;
using System.Drawing.Printing;
using Shop.Api.Data;
using Shop.Api.Models;
using X.PagedList;
using System.Linq;
using Microsoft.Net.Http.Headers;
using Shop.Api.Models.CreateModel;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Enums;
using Shop.Api.Models.Products;
using Shop.Api.Abtracst;
using Microsoft.CodeAnalysis;
using Shop.Api.Repository;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Data;

namespace Shop.Api.Repository
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
            if (product is null)
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
            var products = _dbContext.Products.Where(p => p.IsDeleted == false).AsQueryable();
            #region sort
            if (!String.IsNullOrEmpty(search.sort))
            {
                switch (search.sort)
                {
                    case "Namekey": products = products.OrderBy(x => x.Name); break;
                    case "T": products = products.OrderBy(x => x.Price); break;
                    case "G": products = products.OrderByDescending(x => x.Price); break;
                }
            }
            #endregion
            #region Fillter

            if (search.from is not null)
            {
                products = products.Where(pro => pro.Price >= search.from);
            }
            if (search.to is not null)
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
                                         .Select(pro => _mapper.Map<ProductOnlyDTO>(pro))
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

        public async Task<string> AddProductAsysnc(ProductAddModel product)
        {
            if (product.Image is not null)
            {
                var validExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
                var extension = Path.GetExtension(product.Image.FileName);
                if (!validExtensions.Contains(extension.ToLower()))
                {
                    throw new Exception("Invalid file format. Only JPG, PNG, and GIF are allowed.");
                }
                var imageName = Guid.NewGuid().ToString() + extension;
                var imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "image", "products", imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }

#pragma warning disable CS8629 // Nullable value type may be null.
                var pro = new Product
                {
                    Id = Guid.NewGuid(),
                    Name = product.Name is not null ? product.Name : "Unknow",
                    Price = (double)product.Price,
                    Description = product.Description is not null ? product.Description : "Unknow",
                    Image = imagePath,
                };
#pragma warning restore CS8629 // Nullable value type may be null.

                var i = await _dbContext.Products.AddAsync(pro);
                await _dbContext.SaveChangesAsync();
                return pro.Id.ToString();
            }
            return "Image or category maybe null!!";
        }

        public async Task<string> AddSizeProductAsync(AddSizeModel<StringSize> stringSizes)
        {
            Product? product = await _dbContext.Products.Include(p => p.Sizes).FirstOrDefaultAsync(pro => pro.Id == stringSizes.ProductID);
            if (product is null)
            {
                throw new Exception("No exs product");
            }
            if (stringSizes.StringSize is null)
            {
                return "Size cant be null!!";
            }
            var groupedSizes = stringSizes.StringSize
             .GroupBy(s => s.SizeProduct)
             .Select(g => new StringSize
             {
                 SizeProduct = g.Key,
                 Qty = g.Sum(s => s.Qty)
             })
             .ToList();
            foreach (var i in groupedSizes)
            {
                var existingSize = await _dbContext.Sizes.Include(s => s.Products)
                 .FirstOrDefaultAsync(s => s.size == i.SizeProduct && s.Products.Id == product.Id);
                if (existingSize is null)
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
                else
                {
                    existingSize.Qty += i.Qty;
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

        public Task<string> AddProductAzureAsync(ProductAddModel product)
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

            if (szUpdate is null)
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
            if (szUpdate is null)
            {
                return 0;
            }
            return szUpdate.Qty - quanlity;
        }

        public async Task<bool> SetProductAsync(ProductAddModel product)
        {
            Product? productModifi = _dbContext.Products.FirstOrDefault(p => p.Id == product.id);
            if (productModifi is null)
            {
                return false;
            }
            string? imagePath = null;
            if (product.Image is not null)
            {
                if (!string.IsNullOrEmpty(productModifi.Image))
                {
                    var oldImagePath = Path.Combine(productModifi.Image);
                    if (File.Exists(oldImagePath))
                    {
                        File.Delete(oldImagePath);
                    }
                }
                var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "image", "products", imageName);
                using (var stream = new FileStream(imagePath, FileMode.Create))
                {
                    await product.Image.CopyToAsync(stream);
                }
            }
            productModifi.Price = product.Price ?? productModifi.Price;
            productModifi.Name = product.Name ?? productModifi.Name;
            productModifi.Image = imagePath ?? productModifi.Image;
            productModifi.Description = product.Description ?? productModifi.Description;
            _dbContext.Entry(productModifi).State = EntityState.Modified;
            try { await _dbContext.SaveChangesAsync(); return true; } catch { return false; }
        }
        public async Task<bool> Set2ProductAsync(ProductAddModel product)
        {
            using var transaction = _dbContext.Database.BeginTransaction(IsolationLevel.Serializable);
            try
            {
                Product? productModifi = _dbContext.Products.FirstOrDefault(p => p.Id == product.id);
                if (productModifi is null)
                {
                    return false;
                }
                string? imagePath = null;
                if (product.Image is not null)
                {
                    if (!string.IsNullOrEmpty(productModifi.Image))
                    {
                        var oldImagePath = Path.Combine(productModifi.Image);
                        if (File.Exists(oldImagePath))
                        {
                            File.Delete(oldImagePath);
                        }
                    }
                    var imageName = Guid.NewGuid().ToString() + Path.GetExtension(product.Image.FileName);
                    imagePath = Path.Combine(Directory.GetCurrentDirectory(), "wwroot", "image", "products", imageName);
                    using (var stream = new FileStream(imagePath, FileMode.Create))
                    {
                        await product.Image.CopyToAsync(stream);
                    }
                }
                productModifi.Price = product.Price ?? productModifi.Price;
                productModifi.Name = product.Name ?? productModifi.Name;
                productModifi.Image = imagePath ?? productModifi.Image;
                productModifi.Description = product.Description ?? productModifi.Description;
                _dbContext.Entry(productModifi).State = EntityState.Modified;
                _dbContext.SaveChanges();
                transaction.Commit();
            }
            catch (Exception)
            {
                transaction.Rollback();
                throw;
            }
            return true;
        }
    }
}
