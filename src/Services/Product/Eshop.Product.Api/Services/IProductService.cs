using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Shared.Common;
using System.Threading.Tasks;

namespace Eshop.Product.Api.Services
{
    public interface IProductService
    {
        Task<Result<ProductCreated>> GetProduct(string id);
        Task<Result<ProductCreated>> UpsertProduct(CreateProduct product);
        Task<Result<bool>> DeleteProduct(string productId);
    }
}
