using Eshop.Infrastructure.Commands.Product;
using Eshop.Shared.Constants;
using MassTransit;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        private readonly IBusControl _bus;
        public ProductController(IBusControl bus)
        {
            _bus = bus;
        }

        [HttpGet(Name = "GetProduct")]
        public async Task<IActionResult> GetProduct()
        {
            await Task.CompletedTask;
            return Ok("Get product called");
        }    

        [HttpPost("upsert", Name = "CreateOrUpdateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct product)
        {
            var uri = new Uri($"rabbitmq://localhost/{EventBusQueueNames.ProductQueueNames.CreateOrUpdate}");
            var endpoint = await _bus.GetSendEndpoint(uri);
            await endpoint.Send(product);

            return Accepted("Prodcut created");
        }
    }
}
