using AutoMapper;
using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Share.Message;
using Shop.Api.Abtracst;
using Shop.Api.Models;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;
using System.Collections.Generic;
using Shop.Api.Data;
using Shop.Api.Enums;
using Shop.Api.Models;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Shop.Api.Repository
{
    public class OrderResponsitory : IOrderServices
    {
        private readonly NewDBContext _dbContext;
        private readonly IUserServices _userServices;
        private readonly IProductServices _productServices;
        private readonly IPushlishService<ProductSend> _pushlishService;
        private readonly IMapper _mapper;
        private readonly IImageServices _imageServices;
        public OrderResponsitory(IImageServices imageServices, IMapper mapper, NewDBContext dbContext, IUserServices userServices, IProductServices productServices, IPushlishService<ProductSend> pushlishService)
        {
            _imageServices = imageServices;
            _dbContext = dbContext;
            _userServices = userServices;
            _productServices = productServices;
            _pushlishService = pushlishService;
            _mapper = mapper;
        }

        public async Task<IList<BillDTO>> GetBillsAsync(string email)
        {
            UserApp customer = await _userServices.GetUserByEmailAsync(email);
            var customerId = customer.Id;

            var result = _dbContext.Bills
                .Where(b => b.UserApp.Id == customerId)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                .ToList();

            IList<BillDTO> results = new List<BillDTO>();
            foreach (var bill in result)
            {
                foreach (var billDetail in bill.BillDetails)
                {
                    var dto = new BillDTO
                    {
                        Name = billDetail.Product.Name,
                        Image = billDetail.Product.Image,
                        OderDate = bill.OderDate,
                        Price = billDetail.Price,
                        Size = billDetail.Size,
                        Total = billDetail.Totals,
                        IM = await _imageServices.ParseAsync(billDetail.Product.Image)
                    };
                    results.Add(dto);
                }
            }
            return results;
        }

        public async Task<double> GetPriceAsync(IList<ProductsRequest?> products)
        {
            double? total = default(double);
            foreach (ProductsRequest? product in products)
            {
                if (product == null) continue;
                var productFind = await _dbContext.Products.FindAsync(product?.id);
                var qty = await _productServices.CheckQtyAsync(product.Qty, product.id, product.Size);
                if (qty < 0) return 0.0;
                total += productFind?.Price * product?.Qty;
            }
            return (double)total;
        }

        public async Task<OrderLog> OrderAsync(OrderRequest request, string email)
        {
            //List Product fail to order
            UserApp? custommer = await _userServices.GetUserByEmailAsync(email);
            Bill bill = new Bill
            {
                Id = Guid.NewGuid(),
                UserApp = custommer,
                Adress = request.Adress ?? custommer.Adress ?? "Unknow",
                Phone = request.Phone ?? custommer.PhoneNumber ?? "Unknow",
                OderDate = DateTime.Now,
                BillDetails = new List<BillDetail>()
            };
            //Check product qty in DB
#pragma warning disable CS8602 // Dereference of a possibly null reference.
            foreach ( var Order in request.Products)
            {
                if (Order == null)
                {
                    continue;
                }
                var product = _dbContext?.Products
                .Join(_dbContext.Sizes.Where(s => s.size == Order.Size),
                p => p.Id,
                s => s.Products.Id,
                (p, s) => new { Product = p, Size = s })
               .Where(ps => ps.Product.Id == Order.id)
               .Select(ps => ps.Product);
                Product? pr = product?.FirstOrDefault();
                if (pr == null)
                {
                    return new OrderLog
                    {
                        Message = new List<string> { $"Fail to order item {await _productServices.GetProductName(Order.id)} với size là {Order.Size}" },
                        Status = false
                    };
                }
                else
                {
                    BillDetail billDetail = new BillDetail
                    {
                        Id = Guid.NewGuid(),
                        Product = pr,
                        Size = Order.Size ?? "No",
                        Totals = Order.Qty,
                        Bill = bill,
                        Price = pr.Price * Order.Qty
                    };
                    var ok = await _productServices.CheckQtyAsync(Order.Qty, pr.Id, Order.Size ?? "NO");
                    if (ok < 0)
                    {
                        return new OrderLog
                        {
                            Message = new List<string> { $"Fail to order item {await _productServices.GetProductName(Order.id)} với size là {Order.Size} chỉ còn {ok + Order.Qty}" },
                            Status = false
                        };
                    }
                    await _dbContext.BillDetails.AddAsync(billDetail);
                }
            }
            foreach (var Order in request.Products)
            {
                await _productServices.UpdateQuantySizeAsync(Order.Qty, Order.id, Order.Size ?? "NO");
            }
            try
            {
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }

            await _pushlishService.PushlishAsync(new ProductSend
            {
                Email = bill.UserApp.Email,
                GuidId = Guid.NewGuid(),
                Name = bill.UserApp.Name
            });

            return new OrderLog { Status = true };
        }

    }
}
