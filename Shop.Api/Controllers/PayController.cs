using Microsoft.AspNetCore.Mvc;
using Shop.Api.Data;
using Microsoft.AspNetCore.Authorization;
using Shop.Api.Abtracst;
using Shop.Api.Models.Order;
using Shop.Api.Models.ListLog;
using Shop.Api.VNPay;
using Shop.Api.Models;

namespace Shop.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayController : ControllerBase
    {
        private readonly IHttpContextAccessor _contextAccessor;
        private readonly IPayService _payService;
        private readonly IConfiguration _configuration;
        private readonly NewDBContext _dbContext;
        public PayController(NewDBContext dbContext, IConfiguration configuration, IPayService payService, IHttpContextAccessor contextAccessor)
        {
            _contextAccessor = contextAccessor;
            _payService = payService;
            _configuration= configuration;
            _dbContext= dbContext;  
        }
        [HttpPost]
        [Route("/Pay")]
        [Authorize(AuthenticationSchemes = "Bearer")]
        public async Task<IActionResult> PayAsync([FromBody] PayModel pay)
        {
            var user = HttpContext.Items["User"] as UserApp;
            var result = await _payService.GetUrlPayAsync(pay);
            return Ok(result);
        }

        [HttpGet]
        [Route("/Accept/Pay")]
        public async Task<IActionResult> AcceptPayAsync([FromQuery] RequestAcceptPay Request )
        {
            string returnContent = string.Empty;
            var vnpCofig = _configuration.GetSection("VNPayCofiguration").Get<VNPayCofiguration>();
            if (Request is not null)
            {
                string? vnp_HashSecret = vnpCofig.Vnp_HashSecret; //Secret key
                VnPayLibrary vnpay = new VnPayLibrary();
         
                //Lay danh sach tham so tra ve tu VNPAY
                //vnp_TxnRef: Ma don hang merchant gui VNPAY tai command=pay    
                //vnp_TransactionNo: Ma GD tai he thong VNPAY
                //vnp_ResponseCode:Response code from VNPAY: 00: Thanh cong, Khac 00: Xem tai lieu
                //vnp_SecureHash: HmacSHA512 cua du lieu tra ve

                Guid orderId = new Guid(Request.vnp_TxnRef);
                long vnp_Amount = Convert.ToInt64(Request.vnp_Amount) / 100;
                long vnpayTranId = Convert.ToInt64(Request.vnp_TransactionNo);
                string? vnp_ResponseCode = Request.vnp_ResponseCode;
                string? vnp_TransactionStatus = Request.vnp_TransactionStatus;
                String vnp_SecureHash = Request.vnp_SecureHash;
                bool checkSignature = vnpay.ValidateSignature(vnp_SecureHash, vnp_HashSecret);
                if (checkSignature)
                {
                    var order = _dbContext.Bills.FirstOrDefault(b => b.Id == orderId);
                    if (order != null)
                    {
                        if (vnp_Amount != 0)
                        {
                            if (order.Status == "chờ thanh toán")
                            {
                                if (vnp_ResponseCode == "00" && vnp_TransactionStatus == "00")
                                {
                                    //Thanh toan thanh cong
                                  
                                    order.Status = "đã thanh toán";
                                }
                                else
                                {
                                    //Thanh toan khong thanh cong. Ma loi: vnp_ResponseCode
                                    //  displayMsg.InnerText = "Có lỗi xảy ra trong quá trình xử lý.Mã lỗi: " + vnp_ResponseCode;
                  
                                    order.Status = "thanh toán thất bại";
                                }

                                //Thêm code Thực hiện cập nhật vào Database 
                                //Update Database

                                returnContent = "{\"RspCode\":\"00\",\"Message\":\"Confirm Success\"}";
                            }
                            else
                            {
                                returnContent = "{\"RspCode\":\"02\",\"Message\":\"Order already confirmed\"}";
                            }
                        }
                        else
                        {
                            returnContent = "{\"RspCode\":\"04\",\"Message\":\"invalid amount\"}";
                        }
                    }
                    else
                    {
                        returnContent = "{\"RspCode\":\"01\",\"Message\":\"Order not found\"}";
                    }
                }
                else
                {
                    returnContent = "{\"RspCode\":\"97\",\"Message\":\"Invalid signature\"}";
                }
            }
            else
            {
                returnContent = "{\"RspCode\":\"99\",\"Message\":\"Input data required\"}";
            }
            /* try
             {
                 string logPath = _configuration["LogPath"];
                 using (StreamWriter writer = new StreamWriter(logPath, true))
                 {
                     writer.WriteLine($"{DateTime.Now} : {returnContent}");
                 }

             }
             catch(Exception ex)
             {
                 Console.WriteLine("Can't Write");

             }*/
            try
            {
                _dbContext.SaveChanges();
            }
            catch(Exception ex)
            {
                throw new AggregateException(ex);
            }
            return Ok(returnContent);
        }
        [HttpGet]
        [Route("/Hello")]
        public async Task<IActionResult> GetName()
        {
            return Ok("Hello");
        }
    }
    
}