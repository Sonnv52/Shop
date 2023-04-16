﻿using Shop.Api.Data;
using Shop.Api.Models.Products;
using Shop.Api.Data;

namespace Shop.Api.Models.CreateModel
{
    public class ProductAdd
    {
        public Guid? id { get; set; }
        public string? Name { get; set; }
        public double? Price { get; set; }
        public string? Description { get; set; }
        public IFormFile? Image { get; set; }
    }
}
