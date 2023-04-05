using MassTransit;
using Microsoft.AspNetCore.Identity;
using Microsoft.DotNet.Scaffolding.Shared.Messaging;
using Microsoft.EntityFrameworkCore;
using Shop.Api.Abtracst;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;
using Test.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Shop.Api.Repository
{
    public class OrderResponsitory : IOrderServices<string>
    {
        private readonly NewDBContext _dbContext;
        private readonly IPublishEndpoint _publishEndpoint;
        private readonly IUserServices _userServices;
        private readonly IProductServices _productServices;
        public OrderResponsitory(NewDBContext dbContext, IUserServices userServices, IProductServices productServices, IPublishEndpoint publishEndpoint)
        {
            _dbContext = dbContext;
            _userServices = userServices;
            _productServices = productServices;
            _publishEndpoint = publishEndpoint;
        }

        public async Task<double> GetPriceAsync(IList<ProductsRequest?> products)
        {
            double? total = default(double);
            foreach (ProductsRequest? product in products)
            {
                if (product == null) continue;
                var productFind = await _dbContext.Products.FindAsync(product?.id);
                var qty = await _productServices.CheckQtyAsync(product.Qty, product.id, product.Size);
                if (qty <0 ) return 0.0;
                total += productFind?.Price * product?.Qty;
            }
            return (double)total;
        }

        public async Task<OrderLog> OrderAsync(OrderRequest request, string email)
        {
            //List Product fail to order
            var logFail = new OrderLog();
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
            foreach (var Order in request.Products)
            {
                if(Order != null)
                {
                    continue;
                }
                var product = _dbContext?.Products
                .Join(_dbContext.Sizes.Where(s => s.Qty >= Order.Qty && s.size == Order.Size),
                p => p.Id,
                s => s.Products.Id,
                (p, s) => new { Product = p, Size = s })
               .Where(ps => ps.Product.Id == Order.id)
               .Select(ps => ps.Product);
                Product? pr = product?.FirstOrDefault();
                if (pr == null)
                {
#pragma warning disable CS8602 // Dereference of a possibly null reference.
                    logFail?.Message?.Add( $"Fail to order item{await _productServices.GetProductName(Order.id)}");
#pragma warning restore CS8602 // Dereference of a possibly null reference.
                    return logFail ?? new OrderLog();
                }
                else
                {
                    BillDetail billDetail = new BillDetail
                    {
                        Id = Guid.NewGuid(),
                        Product = pr,
                        Size = Order.Size ?? "No",
                        Totals = Order.Qty,
                        Bill = bill
                    };
                    try
                    {
                        var ok = await _productServices.UpdateQuantySizeAsync(Order.Qty, pr.Id, Order.Size ?? "NO");
                        if (!ok)
                        {
                            return new OrderLog
                            {
                                Message = new List<string> { $"Fail to order item{await _productServices.GetProductName(Order.id)}" }
                            };
                        }
                        await _dbContext.BillDetails.AddAsync(billDetail);
                    }
                    catch(Exception ex)
                    {
                        throw new Exception(ex.ToString());
                    }
                }
            }
            try
            {
                await _dbContext.Bills.AddAsync(bill);
                await _dbContext.SaveChangesAsync();
            }catch(Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            return new OrderLog { Status = true };
        }

    }
}
