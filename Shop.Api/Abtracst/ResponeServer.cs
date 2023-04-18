namespace Shop.Api.Abtracst
{
    public class ResponeServer<T>
    {
        public bool Success { get; set; }
        public string? Message { get; set; }
        public T? Data { get; set; }
    }
}
