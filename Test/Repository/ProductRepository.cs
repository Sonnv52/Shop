using AutoMapper;
using Test.Data;
using Test.Models;

namespace Test.Repository
{
    public class ProductRepository : IProductServices
    {
        private readonly NewDBContext _dbContext;
        private readonly IMapper _mapper;
        public ProductRepository(NewDBContext dbContext, IMapper mapper)
        {
            _dbContext = dbContext;
            _mapper = mapper;
        }
        public async Task<IEnumerable<ProductT>> GetProductAsync(SearchModel search, Paging paging)
        {
            var products = _dbContext.Products.AsQueryable();
            #region sort
            if (!String.IsNullOrEmpty(search.sort))
            {
                switch (search.sort)
                {
                    case "Namekey": products = products.OrderBy(x => x.Name); break;
                    case "Price_up": products = products.OrderBy(x => x.Price); break;
                    case "Price_down": products = products.OrderBy(x => x.Price).Reverse(); break;
                }
            }
            #endregion

            if (!String.IsNullOrEmpty(search.key))
            {
                IQueryable<Product> result = products.Where(pr => pr.Name.Contains(search.key) || pr.Type.Name.Contains(search.key) ||
                pr.Type.Id.ToString().Equals(search.key));
                IEnumerable<ProductT> returns = result.Select(pro => _mapper.Map<ProductT>(pro));
                return returns;
            }
            else
            {
                IEnumerable<ProductT> reuturns = products.Select(pr => _mapper.Map<ProductT>(pr));
                return reuturns;
            }

        }
    }
}
