﻿global using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Shop.Api.Data;

namespace Shop.Api.Data
{
    public class NewDBContext : IdentityDbContext<UserApp>
    {
        public NewDBContext(DbContextOptions<NewDBContext> options) : base(options)
        {
        }
        public DbSet<Bill> Bills { get; set; }
        public DbSet<BillDetail> BillDetails { get; set; }
        public DbSet<Product> Products { get; set; }
        public DbSet<ImageProducts> ImageProducts { get; set; }
        public DbSet<Size> Sizes { get; set; }  
    }
}
