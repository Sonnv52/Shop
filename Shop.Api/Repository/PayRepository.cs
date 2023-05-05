using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Shop.Api.Abtracst;
using Shop.Api.Data;
using Shop.Api.Helper;
using Shop.Api.Models;
using Shop.Api.Models.Order;
using Shop.Api.VNPay;
using System.Security.Cryptography;
using System.Text;

namespace Shop.Api.Repository
{
    public class PayRepository : IPayService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<PayRepository> _logger;
        private readonly string _keyDecrypt;
        public PayRepository(ILogger<PayRepository> logger, IConfiguration configuration)
        {
            _configuration = configuration;
            _keyDecrypt = _configuration["Key_Decrypt"] ?? "";
            _logger = logger;
        }
        public async Task<string> GetUrlPayAsync(PayModel pay)
        {
            if (string.IsNullOrEmpty(pay.Amount) || string.IsNullOrEmpty(pay.PaymentMthods))
            {
                _logger.LogError("Pay Method and Amount cant be null");
                throw new Exception("Pay Method and Amount cant be null");
            }
            //DeCrypt Here
            var payi = pay.PaymentMthods.DecryptString();
            var amount = Task.Run(() => DeCrypt(pay.Amount));
            var payMethod = Task.Run(() => DeCrypt(pay.PaymentMthods));
            await Task.WhenAll(amount, payMethod);
            if (decimal.TryParse(amount.Result, out decimal money) && int.TryParse(payMethod.Result, out int method))
            {
                var url = Pay(method, money, pay.OrderID);
                return url;
            }
            return "";
        }

        public string Pay(int phuongthuc, decimal money, Guid orderID)
        {
            var emailCofig = _configuration.GetSection("VNPayCofiguration").Get<VNPayCofiguration>();
            string? vnp_Returnurl = emailCofig.Vnp_Returnurl; //URL nhan ket qua tra ve 
            string? vnp_Url = emailCofig.Vnp_Url; //URL thanh toan cua VNPAY 
            string? vnp_TmnCode = emailCofig.Vnp_TmnCode; //Ma định danh merchant kết nối (Terminal Id)
            string? vnp_HashSecret = emailCofig.Vnp_HashSecret; //Secret Key
           /* OrderInfo order = new OrderInfo();
            order.OrderId = DateTime.Now.Ticks; // Giả lập mã giao dịch hệ thống merchant gửi sang VNPAY
            order.Amount = money; // Giả lập số tiền thanh toán hệ thống merchant gửi sang VNPAY 100,000 VND
            order.Status = "0"; //0: Trạng thái thanh toán "chờ thanh toán" hoặc "Pending" khởi tạo giao dịch chưa có IPN
            order.CreatedDate = DateTime.Now;*/
            //Save order to db

            //Build URL for VNPAY
            VnPayLibrary vnpay = new VnPayLibrary();

            vnpay.AddRequestData("vnp_Version", VnPayLibrary.VERSION);
            vnpay.AddRequestData("vnp_Command", "pay");
            vnpay.AddRequestData("vnp_TmnCode", vnp_TmnCode ?? "");
            vnpay.AddRequestData("vnp_Amount", (money * 100).ToString()); //Số tiền thanh toán. Số tiền không mang các ký tự phân tách thập phân, phần nghìn, ký tự tiền tệ. Để gửi số tiền thanh toán là 100,000 VND (một trăm nghìn VNĐ) thì merchant cần nhân thêm 100 lần (khử phần thập phân), sau đó gửi sang VNPAY là: 10000000
            if (phuongthuc == 1)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNPAYQR");
            }
            else if (phuongthuc == 2)
            {
                vnpay.AddRequestData("vnp_BankCode", "VNBANK");
            }
            else if (phuongthuc == 3)
            {
                vnpay.AddRequestData("vnp_BankCode", "INTCARD");
            }

            vnpay.AddRequestData("vnp_CreateDate", DateTime.Now.ToString("yyyyMMddHHmmss"));
            vnpay.AddRequestData("vnp_CurrCode", "VND");
            vnpay.AddRequestData("vnp_IpAddr", "");
            vnpay.AddRequestData("vnp_Locale", "vn");
            /*if (locale_Vn.Checked == true)
            {
                vnpay.AddRequestData("vnp_Locale", "vn");
            }
            else if (locale_En.Checked == true)
            {
                vnpay.AddRequestData("vnp_Locale", "en");
            }*/
            vnpay.AddRequestData("vnp_OrderInfo", "Thanh toan don hang:" + orderID);
            vnpay.AddRequestData("vnp_OrderType", "other"); //default value: other

            vnpay.AddRequestData("vnp_ReturnUrl", vnp_Returnurl ?? "");
            vnpay.AddRequestData("vnp_TxnRef", orderID.ToString()); // Mã tham chiếu của giao dịch tại hệ thống của merchant. Mã này là duy nhất dùng để phân biệt các đơn hàng gửi sang VNPAY. Không được trùng lặp trong ngày

            //Add Params of 2.1.0 Version
            //Billing

            string paymentUrl = vnpay.CreateRequestUrl(vnp_Url ?? "", vnp_HashSecret ?? "");
            return paymentUrl;
        }

        private string DeCrypt(string cipherText)
        {
            try
            {
                byte[] cipherBytes = Convert.FromBase64String(cipherText);
                using (Aes aes = Aes.Create())
                {
                    aes.Key = Encoding.UTF8.GetBytes(_keyDecrypt);
                    aes.IV = new byte[16];
                    using (MemoryStream ms = new MemoryStream())
                    {
                        using (CryptoStream cs = new CryptoStream(ms, aes.CreateDecryptor(), CryptoStreamMode.Write))
                        {
                            cs.Write(cipherBytes, 0, cipherBytes.Length);
                            cs.Close();
                        }
                        byte[] decryptedBytes = ms.ToArray();
                        return Encoding.UTF8.GetString(decryptedBytes);
                    }
                }
            }
            catch
            {
                return "Cant convert!!";
            }
        }
    }
}