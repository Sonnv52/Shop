namespace Shop.Api.Models.CreateModel
{
    public class AddSize<T>: List<T> where T : class
    {
        public IList<T> StringSize { get; set; }
        public Guid ProductID { get; set; }
    }
}
