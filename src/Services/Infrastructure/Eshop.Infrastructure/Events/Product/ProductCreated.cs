using System;

namespace Eshop.Infrastructure.Events.Product
{
    public class ProductCreated
    {
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
