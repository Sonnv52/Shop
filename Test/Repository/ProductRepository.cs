using AutoMapper;
using System.Drawing.Printing;
using Test.Data;
using Test.Models;
using X.PagedList;
using System.Linq;
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

        public Task<Product> GetOneProductAsync(Guid id)
        {
            throw new NotImplementedException();
        }

        public async Task<PageProduct> GetProductAsync(SearchModel search, PagingSearch paging)
        {
            var products = _dbContext.Products.AsQueryable();
            #region sort
            if (!String.IsNullOrEmpty(search.sort))
            {
                switch (search.sort)
                {
                    case "Namekey": products = products.OrderBy(x => x.Name); break;
                    case "Price_up": products = products.OrderBy(x => x.Price); break;
                    case "Price_down": products = products.OrderByDescending(x => x.Price); break;
                }
            }
            #endregion
            #region Fillter

            if (search.from != null)
            {
                products = products.Where(pro => pro.Price >= search.from);
            }
            if (search.to != null)
            {
                products = products.Where(pro => pro.Price <= search.to);
            }
            #endregion
            if (!String.IsNullOrEmpty(search.key))
            {
                products = products.Where(pr => pr.Name.Contains(search.key) || pr.Type.Name.Contains(search.key) ||
                pr.Type.Id.ToString().Equals(search.key));

            }

            IEnumerable<ProductT> returns = products.Select(pro => _mapper.Map<ProductT>(pro));
            var total = returns.Count();
            returns = returns.ToPagedList(paging.PageIndex, paging.PageSize);
            return new PageProduct
            {

                Products = returns.ToList(),
                totalPage = total
            };
        }
    }
}
