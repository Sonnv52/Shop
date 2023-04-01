namespace Shop.Api.Abtracst
{
    public abstract class ResponeModel<T, K>
    {
        public IList<T>? Message { get; set; }
        public K? Status { get; set; }
        public abstract T Log(string message);
    }
}
