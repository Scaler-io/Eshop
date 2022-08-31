using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Shared.Common;
using MassTransit;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Services.Product
{
    public interface IProductEvent
    {
        Task PublishUpsertEventAsync(CreateProduct request);
        Task<Response<Result<ProductCreated>>> ConsumeProductAsync(string productId);
        Task PublishDeleteEventAsync(string productId);
    }
}
