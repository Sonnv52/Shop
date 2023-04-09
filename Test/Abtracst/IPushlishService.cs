namespace Shop.Api.Abtracst
{
    public interface IPushlishService<T> where T : class
    {
        public Task PushlishAsync(T data);
    }
}
