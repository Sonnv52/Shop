namespace Shop.Api.Models.CreateModel
{
    public class AddSizeModel<T>
    {
        public IList<T>? StringSize { get; set; }
        public Guid ProductID { get; set; }
    }
}
