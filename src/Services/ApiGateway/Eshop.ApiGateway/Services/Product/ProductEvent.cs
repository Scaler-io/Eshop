using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Infrastructure.Query.Product;
using Eshop.Shared.Common;
using Eshop.Shared.Constants;
using Eshop.Shared.Extensions;
using MassTransit;
using Serilog;
using System;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Services.Product
{
    public class ProductEvent : IProductEvent
    {
        private readonly ILogger _logger;
        private readonly IBusControl _bus;
        private readonly IRequestClient<GetProductById> _requestClient;

        public ProductEvent(ILogger logger, IBusControl bus, IRequestClient<GetProductById> requestClient)
        {
            _logger = logger;
            _bus = bus;
            _requestClient = requestClient;
        }

        public async Task PublishUpsertEventAsync(CreateProduct request)
        {
            _logger.Here().MethodEnterd();
            var endpoint = await GetEventBusURI(EventBusQueueNames.ProductQueueNames.CreateOrUpdate);
            await endpoint.Send(request);
            _logger.Here().MethodExited();
        }
        public async Task<Response<Result<ProductCreated>>> ConsumeProductAsync(string productId)
        {
            _logger.Here().MethodEnterd();
            var request = new GetProductById { ProductId = productId };
            var response = await _requestClient.GetResponse<Result<ProductCreated>>(request);          
            _logger.Here().ForContext(LoggerConstants.CorrelationId, response.CorrelationId?.ToString())
                         .ForContext(LoggerConstants.RequestId, response.RequestId.ToString())
                         .ForContext(LoggerConstants.MessageId, response.MessageId.ToString())
                         .Information("Product found with {@productId}", productId);

            _logger.Here().MethodExited();
            return response;
        }

        public async Task PublishDeleteEventAsync(string productId)
        {
            _logger.Here().MethodEnterd();
            var request = new DeleteProduct {  ProductId = productId };
            var endpoint = await GetEventBusURI(EventBusQueueNames.ProductQueueNames.Delete);
            await endpoint.Send(request);
            _logger.Here().MethodExited();
        }

        private async Task<ISendEndpoint> GetEventBusURI(string queue)
        {
            var uri = new Uri($"rabbitmq://localhost/{queue}");
            return await _bus.GetSendEndpoint(uri);
        }
    }
}
