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
using StackExchange.Redis;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;
using Microsoft.Extensions.Caching.Distributed;
using System.Text;
using Shop.Api.Models.Page;
using System.Drawing.Printing;
using System.Net.WebSockets;

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
        private readonly IDistributedCache _cache;
        private readonly ILogger<OrderResponsitory> _logger;
        public OrderResponsitory(ILogger<OrderResponsitory> logger, IDistributedCache cache, IImageServices imageServices, IMapper mapper, NewDBContext dbContext, IUserServices userServices, IProductServices productServices, IPushlishService<ProductSend> pushlishService)
        {
            _imageServices = imageServices;
            _dbContext = dbContext;
            _userServices = userServices;
            _productServices = productServices;
            _pushlishService = pushlishService;
            _mapper = mapper;
            _cache = cache;
            _logger = logger;
        }

        public async Task<PagedList<BillAdminDTO>> GetAllBillAsync(int page, int pageSize)
        {
            var bills = _dbContext.Bills.Include(b => b.UserApp);
            var billPage = bills.Select(b => new BillAdminDTO
            {
                Id = b.Id,
                Adress = b.Adress,
                OderDate = b.OderDate,
                Phone = b.Phone,
                Status = b.Status,
                email = b.UserApp.Email,
                Name= b.UserApp.Name
            });
            var billResult = billPage.ToPagedList(page, pageSize);
            return billResult;
        }

        public async Task<BillDetailDTO> GetBillDetailAsync(Guid id)
        {
            var bill = await _dbContext.Bills!.Where(b => b.Id == id).Include(b => b.UserApp).Include(x => x.BillDetails).ThenInclude(bd => bd.Product).FirstOrDefaultAsync();
            if (bill is null)
                return new BillDetailDTO
                {};

            var result = new BillDetailDTO
            {
                GuidId = bill.Id,
                OrderDate = bill.OderDate,
                OrderStatus = bill.Status,
                Email = bill.UserApp.Email,
                Detail = bill!.BillDetails!.Select(b => new DetailBill
                {
                    Price = b.Price?? 0,
                    ProductId = b.Product!.Id,
                    ProductName = b.Product.Name,
                    Qty = b.Totals,
                    IM = _imageServices.Parse(b.Product.Image)
                }).ToList()
            };

            return result;
        }

        public async Task<IList<BillDTO?>> GetBillsAsync(string email)
        {
            UserApp customer = await _userServices.GetUserByEmailAsync(email);
            var customerId = customer.Id;

            var result = _dbContext.Bills
                .Where(b => b.UserApp.Id == customerId)
                .Include(b => b.BillDetails)
                    .ThenInclude(bd => bd.Product)
                .ToList();

            IList<BillDTO?> results = new List<BillDTO?>();
            foreach (var bill in result)
            {
                foreach (var billDetail in bill.BillDetails)
                {
                    if (billDetail.Product is null) continue;
                    var dto = new BillDTO
                    {
                        id = billDetail.Id,
                        Name = billDetail.Product.Name,
                        Image = billDetail.Product.Image,
                        OderDate = bill.OderDate,
                        Price = billDetail.Price,
                        Size = billDetail.Size,
                        Total = billDetail.Totals,
                        Status = bill.Status,
                        IM = await _imageServices.ParseAsync(billDetail.Product.Image)
                    };
                    results.Add(dto);
                }
            }
            return results;
        }

        public async Task<IList<BillList>> GetYourBillAsync(string email)
        {
            UserApp customer = await _userServices.GetUserByEmailAsync(email);
            var bills = _dbContext.Bills.Include(b => b.UserApp).Include(b => b.BillDetails)
                .ThenInclude(b => b.Product).Where(b => b.UserApp.Id == customer.Id).OrderByDescending(b => b.OderDate);
            if(bills is null)
            {
                return new List<BillList>();
            }
            var billPage = bills.Select(b => new BillList(b)
            ).ToList();
            return billPage;

        }
        public async Task<double> GetPriceAsync(IList<ProductsRequest?> products)
        {
            double? total = default(double);
            foreach (ProductsRequest? product in products)
            {
                if (product is null) continue;
                var productFind = await _dbContext.Products.FindAsync(product?.id);
                var qty = await _productServices.CheckQtyAsync(product.Qty, product.id, product.Size);
                if (qty < 0) return 0.0;
                total += productFind?.Price * product?.Qty;
            }
            return (double)total;
        }

        public async Task<OrderLog> OrderAsync(OrderRequest request, string email, CancellationToken token)
        {
            IList<byte[]?> Image = new List<byte[]?>();
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
            foreach (var Order in request.Products!)
            {
                if (Order is null)
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
                if (pr is null)
                {
                    return new OrderLog
                    {
                        Message = new List<string> { $"Fail to order item {await _productServices.GetProductName(Order.id)} với size là {Order.Size}" },
                        Status = false
                    };
                }
                else
                {
                    //Get list image
                    string path = product!.Select(p => Path.Combine(p.Image ?? "")).FirstOrDefault()!.ToString();
                    Image.Add(await System.IO.File.ReadAllBytesAsync(path));
                    //Create billDetail
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
                    await _dbContext!.BillDetails.AddAsync(billDetail);
                }
            }
            foreach (var Order in request.Products)
            {
                await _productServices.UpdateQuantySizeAsync(Order.Qty, Order.id, Order.Size ?? "NO");
            }
            try
            {
                if (!token.IsCancellationRequested)
                    await _dbContext.SaveChangesAsync();
                else
                    _logger.LogInformation($"{custommer.UserName} cancel when order at {DateTime.Now.ToString()})!!");
            }
            catch (Exception ex)
            {
                throw new Exception(ex.ToString());
            }
            /*IList<string> keyPushlish = new List<string>();
            foreach (var i in Image)
            {
                var imKey = new StringBuilder($"image:{Guid.NewGuid()}");
                if (i is null) continue;
                try
                {
                    await _cache.SetStringAsync(imKey.ToString(), Convert.ToBase64String(i), new DistributedCacheEntryOptions
                    {
                        AbsoluteExpirationRelativeToNow = TimeSpan.FromDays(1)
                    });
                    
                }
                catch (Exception ex)
                {
                    _logger.LogCritical(ex.ToString());
                }
            }*/

            return new OrderLog { Status = true };
        }

        public async Task<bool> SetBillAsync(IList<SetBillRequest> setBills)
        {
           foreach(var setBill in setBills)
            {
                if(setBill is null) continue;
                var bill = await _dbContext.Bills.FirstOrDefaultAsync(b => b.Id == setBill.id);
                if(bill is null) continue;
                if (setBill.status == (int)OrderStatus.Shipping)
                    bill.Status = "Đang giao hàng";
                else if (setBill.status == (int)OrderStatus.Canceled)
                    bill.Status = "Đã hủy đơn";
                else if (setBill.status == (int)OrderStatus.Success)
                    bill.Status = "Đã giao hàng";
                _dbContext.Entry(bill).State = EntityState.Modified;  
            }
            try
            {
                _dbContext.SaveChanges();
            }
            catch (DbUpdateConcurrencyException e)
            {
                _logger.LogCritical(e.ToString());
                return false;
            }
            return true;
        }

        public async Task<BillDetailDTO> GetYourBillDetaillAsync(string email, Guid idBill)
        {
            var custommer = await _userServices.GetUserByEmailAsync(email);
            var bill = await _dbContext.Bills!.Where(b => b.Id == idBill).Include(b => b.UserApp).Include(x => x.BillDetails).ThenInclude(bd => bd.Product).FirstOrDefaultAsync();
            if (bill is null)
                return new BillDetailDTO { };

            if (bill.UserApp.Id != custommer.Id)
            {
                _logger.LogCritical("Get bill not by customer!!");
                return new BillDetailDTO { };
            }
            if(bill.BillDetails is null) {
                return new BillDetailDTO { };
            }
#pragma warning disable CS8619 // Nullability of reference types in value doesn't match target type.
            var result = new BillDetailDTO
            {
                GuidId = bill.Id,
                OrderDate = bill.OderDate,
                OrderStatus = bill.Status,
                Email = bill.UserApp.Email,
                Detail = bill.BillDetails.Select(b => new DetailBill
                {
                    Price = b.Price ?? 0,
                    ProductId = b.Product!.Id,
                    ProductName = b.Product.Name,
                    Qty = b.Totals,
                    IM = _imageServices.Parse(b.Product.Image)
                }).ToList()
            };
#pragma warning restore CS8619 // Nullability of reference types in value doesn't match target type.
            return result;
        }
    }
}
