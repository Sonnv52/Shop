namespace Shop.Api.Abtracst
{
    public abstract class ResponeModel<T>
    {
        public string? Message { get; set; }
        public string? Status { get; set; }
        public abstract T Log(string message);
    }
}
