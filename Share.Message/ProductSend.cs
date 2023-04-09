﻿namespace Share.Message
{
    public class ProductSend
    {
        public Guid GuidId { get; set; }
        public string? Email { get; set; }
        public string? Name { get; set; }
        public IList<Products>? products { get; set;}
        public double Total { get; set; } = 0;

    }
    public class Products
    {
        public string? Name { get; set; }
        public string Size { get; set; } = string.Empty;
    }
}