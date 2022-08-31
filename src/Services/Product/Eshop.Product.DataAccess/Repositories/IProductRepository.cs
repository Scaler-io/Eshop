using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace Eshop.Product.DataAccess.Repositories
{
    public interface IProductRepository
    {
        Task<ProductCreated> GetProduct(string productId);
        Task<ProductCreated> Upsert(CreateProduct product);
        Task<IEnumerable<ProductCreated>> GetProductByPredicate(Expression<Func<CreateProduct, bool>> predicate);
        Task<bool> DeleteProduct(string productId);
    }
}
