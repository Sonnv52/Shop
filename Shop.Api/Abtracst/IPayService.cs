namespace Shop.Api.Abtracst
{
    public interface IPayService
    {
        public Task<string> GetUrlPayAsync(double tien);
    }
}
