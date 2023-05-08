using MassTransit;
using Org.BouncyCastle.Asn1.Cmp;
using Shop.Api.Data;
using System.Globalization;

namespace Shop.Api.Models.Order
{
    public class BillList
    {
        public Guid Id { get; set; }
        public string? Adress { get; set; }
        public string? Phonenumber { get; set; }
        public string? OrderDate { get; set; }
        public string? Status { get; set; }
        public double? Price { get; set; }  
        public string? PayStatus { get; set; }  
        public string? SumaryName { get; set; }
        public IEnumerable<byte[]>? IM { get; set; }
        public BillList(Bill  bill, IEnumerable<byte[]>? iM)
        {
            Id = bill.Id;
            Adress = bill.Adress;
            Phonenumber = bill.Phone;
            OrderDate = bill.OderDate.ToString("dd/MM/yyyy HH:mm");
            Status = bill.Status;
            PayStatus = bill.PayStatus;
            Price = Math.Round(bill.BillDetails?.Sum(bill => bill.Price) ?? 0, 0);
            SumaryName = String.Join(", ", bill.BillDetails.Select(b => $"{b.Product?.Name} Size: {b.Size} số lượng {b.Totals}"));
            IM = iM;
        }
    }
}
