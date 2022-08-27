using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Product.Api.Repositories;
using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using Eshop.Shared.Extensions;
using Serilog;
using System.Threading.Tasks;

namespace Eshop.Product.Api.Services
{
    public class ProductService : IProductService
    {
        private readonly IProductRepository _productRepository;
        private readonly ILogger _logger;

        public ProductService(IProductRepository productRepository, ILogger logger)
        {
            _productRepository = productRepository;
            _logger = logger;
        }
        public async Task<Result<ProductCreated>> UpsertProduct(CreateProduct product)
        {
            _logger.Here().MethodEnterd();
            var dbResult =  await _productRepository.Upsert(product);

            _logger.Here().Information("product db updated {@product}", product);
            _logger.Here().MethodExited();

            return Result<ProductCreated>.Success(dbResult);
        }

        public async Task<Result<ProductCreated>> GetProduct(string id)
        {
            _logger.Here().MethodEnterd();
            var product = await _productRepository.GetProduct(id);

            if(product == null)
            {
                _logger.Here().Warning("{@ErroCode}. No product found with id {@productId}", ErrorCodes.NotFound, id);
                return null;
            }

            _logger.Here().Information("product found {@product}", product);
            _logger.Here().MethodExited();

            return Result<ProductCreated>.Success(product);
        }

        public async Task<Result<bool>> DeleteProduct(string productId)
        {
            _logger.Here().MethodEnterd();
            var isDeleted = await _productRepository.DeleteProduct(productId);

            if (!isDeleted)
            {
                _logger.Here().Warning("{@ErrorCode}. Failed to delete product {@productId}", ErrorCodes.Operationfailed, productId);
                return Result<bool>.Fail(ErrorCodes.Operationfailed, "Failed to delete product");
            }

            _logger.Here().Information("product deleted {@productId}", productId);
            _logger.Here().MethodExited();

            return Result<bool>.Success(true);
        }
    }
}
