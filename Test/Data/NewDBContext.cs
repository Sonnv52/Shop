﻿global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;

namespace Test.Data
{
    public class NewDBContext : IdentityDbContext<UserApp>
    {
        public NewDBContext(DbContextOptions<NewDBContext> options) : base(options)
        {
        }
       public DbSet<Bill> Bills { get; set; }
       public DbSet<BillDetail> BillDetails { get; set; }
      public  DbSet<Product> Products { get; set; }
      public  DbSet<Type> Types { get; set; }
    }
}
