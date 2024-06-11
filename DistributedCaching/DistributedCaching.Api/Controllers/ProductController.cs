using DistributedCaching.Api.Models;
using DistributedCaching.Api.Services;
using Microsoft.AspNetCore.Mvc;

namespace DistributedCaching.Api.Controllers;

[ApiController]
[Route("[controller]")]
public class ProductController(IProductService productService) : ControllerBase
{
    [HttpGet("products")]
    public async Task<IActionResult> Get()
    {
        var products = await productService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("product/id:int")]
    public async Task<IActionResult> GetProduct(int id)
    {
        var product = await productService.GetAsync(id);
        return Ok(product);
    }

    [HttpPost("product")]
    public async Task<IActionResult> AddProduct(ProductCreationDto product)
    {
        await productService.AddSync(product);
        return Ok(Results.Created());
    }
}