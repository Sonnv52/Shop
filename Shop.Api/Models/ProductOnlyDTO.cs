﻿using Shop.Api.Data;

namespace Shop.Api.Models
{
    public class ProductOnlyDTO
    {
        public Guid Id { get; set; }
      
        public string? Name { get; set; }
     
        public double Price { get; set; }
        public string? Description { get; set; }
        public string? Image { get; set; }
        public double? Seleoff { get; set; }
        public byte[]? IM { get; set; }
    }
}
