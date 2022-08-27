using Eshop.Infrastructure.Commands.Product;
using Eshop.Product.Api.Services;
using Eshop.Shared.Extensions;
using Microsoft.AspNetCore.Mvc;
using Serilog;
using System.Threading.Tasks;

namespace Eshop.Product.Api.Controllers.v1
{

    [ApiVersion("1")]
    public class ProductController: BaseApiController
    {
        private readonly IProductService _productService;
        public ProductController(ILogger logger, IProductService productService)
            : base(logger)
        {
            _productService = productService;
        }

        [HttpGet("{id}", Name = "GetProduct")]
        public async Task<IActionResult> GetProduct(string id)
        {
            Logger.Here().MethodEnterd();
            var result = await _productService.GetProduct(id);
            Logger.Here().MethodExited();
            return OkOrFail(result);
        }

        [HttpPost("upsert", Name = "CreateOrUpdateProduct")]
        public async Task<IActionResult> CreateOrUpdateProduct([FromBody] CreateProduct product)
        {
            Logger.Here().MethodEnterd();
            var result = await _productService.UpsertProduct(product);
            Logger.Here().MethodExited();
            return OkOrFail(result);
        }

        [HttpDelete("{id}/delete", Name = "DeleteProduct")]
        public async Task<IActionResult> DeleteProduct([FromRoute] string id)
        {
            Logger.Here().MethodEnterd();
            var result = await _productService.DeleteProduct(id);
            Logger.Here().MethodExited();
            return OkOrFail(result);
        }
    }
}
