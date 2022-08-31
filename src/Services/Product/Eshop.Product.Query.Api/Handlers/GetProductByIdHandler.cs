using Eshop.Infrastructure.Events.Product;
using Eshop.Infrastructure.Query.Product;
using Eshop.Product.DataAccess.Services;
using Eshop.Shared.Common;
using Eshop.Shared.Extensions;
using MassTransit;
using Serilog;
using System.Threading.Tasks;

namespace Eshop.Product.Query.Api.Handlers
{
    public class GetProductByIdHandler : IConsumer<GetProductById>
    {
        private readonly IProductService _productService;
        private readonly ILogger _logger;

        public GetProductByIdHandler(IProductService productService, ILogger logger)
        {
            _productService = productService;
            _logger = logger;
        }

        public async Task Consume(ConsumeContext<GetProductById> context)
        {
            _logger.Here().MethodEnterd();
            var message = await _productService.GetProduct(context.Message.ProductId);
            await context.RespondAsync(message);

            _logger.Here().Information("consumer acknowledged with reposne successfully");
            _logger.Here().MethodExited();
        }
    }
}
