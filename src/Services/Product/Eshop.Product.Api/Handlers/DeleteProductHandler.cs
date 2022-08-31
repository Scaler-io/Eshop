using Eshop.Infrastructure.Commands.Product;
using Eshop.Product.DataAccess.Services;
using Eshop.Shared.Extensions;
using MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace Eshop.Product.Api.Handlers
{
    public class DeleteProductHandler : IConsumer<DeleteProduct>
    {
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public DeleteProductHandler(IProductService productService, ILogger logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<DeleteProduct> context)
        {
            _logger.Here().MethodEnterd();
            await _productService.DeleteProduct(context.Message.ProductId);
            _logger.Here().MethodExited();
        }
    }
}
