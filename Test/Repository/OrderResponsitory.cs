using Microsoft.AspNetCore.Identity;
using Shop.Api.Abtracst;
using Shop.Api.Models.ListLog;
using Shop.Api.Models.Order;
using Test.Data;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace Shop.Api.Repository
{
    public class OrderResponsitory : IOrderServices
    {
        private readonly NewDBContext _dbContext;
        private readonly IUserServices? _userServices;
        private readonly IProductServices? _productServices;

        public OrderResponsitory(NewDBContext? dbContext, IUserServices userServices, IProductServices productServices)
        {
            _dbContext = dbContext;
            _userServices = userServices;
            _productServices = productServices;
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
                Adress = request.Adress ?? custommer.Adress,
                Phone = request.Phone ?? custommer.PhoneNumber,
                OderDate = DateTime.Now,
                BillDetails = new List<BillDetail>()
            };
            //Check product qty in DB
            foreach (var Order in request.Products)
            {
                var product = _dbContext?.Products
                .Join(_dbContext.Sizes.Where(s => s.Qty - Order.Qty >= 0 && s.size == Order.Size),
                p => p.Id,
                s => s.Products.Id,
                (p, s) => new { Product = p, Size = s })
               .Where(ps => ps.Product.Id == Order.id)
               .Select(ps => ps.Product);
                Product? pr = product?.FirstOrDefault();
                if (pr == null)
                {
                    logFail?.Message?.Add( $"Fail to order item{await _productServices.GetProductName(Order.id)}");
                    return logFail;
                }
                else
                {
                    BillDetail billDetail = new BillDetail
                    {
                        Id = Guid.NewGuid(),
                        Product = pr,
                        Size = Order.Size,
                        Totals = Order.Qty,
                        Bill = bill
                    };
                    try
                    {
                        await _dbContext.BillDetails.AddAsync(billDetail);
                        await _productServices.UpdateQuantySizeAsync(Order.Qty, pr.Id, Order.Size);
                    }catch(Exception ex)
                    {
                        throw new Exception(ex.ToString() );
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
