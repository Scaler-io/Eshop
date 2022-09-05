using Eshop.ApiGateway.Services.Product;
using Eshop.Infrastructure.Commands.Product;
using Eshop.Infrastructure.Events.Product;
using Eshop.Shared.Common;
using Eshop.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Net;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Controllers
{
    [Route("product")]
    public class ProductController : BaseApiController
    {
        private readonly IProductEvent _productEvent;

        public ProductController(ILogger logger, IProductEvent productEvent)
            : base(logger)
        {
            _productEvent = productEvent;
        }

        [HttpGet("{id}", Name = "GetProduct")]
        [ProducesResponseType(typeof(ProductCreated), (int)HttpStatusCode.OK)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> GetProduct([FromRoute]string id)
        {
            Logger.Here().MethodEnterd();
            var result = await _productEvent.ConsumeProductAsync(id);
            Logger.MethodExited();
            return OkOrFail(result.Message);
        }    

        [HttpPost("upsert", Name = "CreateOrUpdateProduct")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct product)
        {
            Logger.Here().MethodEnterd();
            await _productEvent.PublishUpsertEventAsync(product);
            Logger.MethodExited();
            return Accepted("Product published successfully");
        }

        [HttpDelete("{id}/delete", Name = "DeleteProduct")]
        [ProducesResponseType(typeof(void), (int)HttpStatusCode.Accepted)]
        [ProducesResponseType(typeof(ApiExceptionResponse), (int)HttpStatusCode.InternalServerError)]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {
            Logger.Here().MethodEnterd();
            await _productEvent.PublishDeleteEventAsync(id);
            Logger.MethodExited();
            return Accepted("Product deleted successfully");
        }
    }
}
