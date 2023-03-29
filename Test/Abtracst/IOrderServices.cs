namespace Shop.Api.Abtracst
{
    public interface IOrderServices
    {
        public Task<Guid> OrderAsync();
    }
}
