namespace Shop.Api.Abtracst
{
    public interface ILog<T>
    {
        public Task TaskLogAsync(T message);
    }
}
