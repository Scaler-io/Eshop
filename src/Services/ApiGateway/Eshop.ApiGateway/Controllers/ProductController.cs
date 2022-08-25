using Eshop.Infrastructure.Commands.Product;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Eshop.ApiGateway.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ProductController : ControllerBase
    {
        [HttpGet(Name = "GetProduct")]
        public async Task<IActionResult> GetProduct()
        {
            await Task.CompletedTask;
            return Ok("Get product called");
        }    

        [HttpPost("Create", Name = "CreateProduct")]
        public async Task<IActionResult> CreateProduct([FromBody] CreateProduct product)
        {
            await Task.CompletedTask;  
            return Accepted("Prodcut created");
        }
    }
}
