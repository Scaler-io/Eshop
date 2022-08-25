using System;

namespace Eshop.Infrastructure.Commands.Product
{
    public class CreateProduct
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ProductDescription { get; set; }
        public string ProductImageUrl { get; set; }
        public decimal ProductPrice { get; set; }
        public Guid CategoryId { get; set; }
    }
}
