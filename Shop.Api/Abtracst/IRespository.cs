namespace Shop.Api.Abtracst
{
    public interface IRepository<T> where T : class
    {
        public Task<T> GetAsync(Guid id);
        public Task<IEnumerable<T>> GetAllAsync();
        public Task<string> AdddAsync(T Add);
        public Task<string> DeleteAsync(int id);
        public Task<string> UpdateAsync(T Update);
    }
}
