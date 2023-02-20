global using Microsoft.EntityFrameworkCore;
namespace Test.Data
{
    public class NewDBContext : DbContext
    {
        public NewDBContext(DbContextOptions<NewDBContext> options) : base(options)
        {
        }
       public DbSet<Bill> Bills { get; set; }
       public DbSet<BillDetail> BillDetails { get; set; }
       public DbSet<Customer> Customers { get; set; }
      public  DbSet<Product> Products { get; set; }
      public  DbSet<Type> Types { get; set; }


    }
}
