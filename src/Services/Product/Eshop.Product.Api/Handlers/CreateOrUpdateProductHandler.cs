using Eshop.Infrastructure.Commands.Product;
using Eshop.Product.Api.Services;
using Eshop.Shared.Extensions;
using MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace Eshop.Product.Api.Handlers
{
    public class CreateOrUpdateProductHandler : IConsumer<CreateProduct>
    {
        private readonly IProductService _productService;
        private readonly ILogger _logger;
        public CreateOrUpdateProductHandler(IProductService productService, ILogger logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<CreateProduct> context)
        {
            _logger.Here().MethodEnterd();
            await _productService.UpsertProduct(context.Message);
            _logger.Here().MethodExited();
        }
    }
}
