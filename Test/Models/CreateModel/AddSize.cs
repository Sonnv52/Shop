namespace Shop.Api.Models.CreateModel
{
    public class AddSize<T>
    {
        public IList<T> StringSize { get; set; }
        public Guid ProductID { get; set; }
    }
}
