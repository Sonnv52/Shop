﻿using Microsoft.Build.Framework;
namespace Shop.Api.Models.Order
{
    public class OrderRequest
    {
        public string? CustomerName { get; set; }
        public string? Adress { get; set; }
        public string? Phone { get; set; }
        public string? PayMethod { get; set; }
        public string? Amonut { get; set; }
        [Required]
        public IList<ProductsRequest?>? Products { get; set; }
    }
}