using DistributedCaching.Api.Context;
using DistributedCaching.Api.Extensions;
using DistributedCaching.Api.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;

namespace DistributedCaching.Api.Services;

public class ProductService(AppDbContext context, ILogger<ProductService> logger, IDistributedCache cache)
    : IProductService
{
    public async Task AddSync(ProductCreationDto request)
    {
        var product = new Product { Name = request.Name, Description = request.Description, Price = request.Price };
        await context.Products.AddAsync(product);
        await context.SaveChangesAsync();
        // invalidate cache for products, as new product is added
        var cacheKey = "products";
        logger.LogInformation("invalidating cache for key: {CacheKey} from cache", cacheKey);
        await cache.RemoveAsync(cacheKey);
    }

    public async Task<Product> GetAsync(int id)
    {
        var cacheKey = $"product:{id}";
        logger.LogInformation("fetching data for key: {CacheKey} from cache", cacheKey);
        var product = await cache.GetOrSetAsync(cacheKey,
            async () =>
            {
                logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database", cacheKey);
                return await context.Products.FindAsync(id)!;
            })!;
        return product!;
    }

    public async Task<List<Product>> GetAllAsync()
    {
        var cacheKey = "products";
        logger.LogInformation("fetching data for key: {CacheKey} from cache", cacheKey);
        var cacheOptions = new DistributedCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(20))
            .SetSlidingExpiration(TimeSpan.FromMinutes(2));
        var products = await cache.GetOrSetAsync(
            cacheKey,
            async () =>
            {
                logger.LogInformation("cache miss. fetching data for key: {CacheKey} from database", cacheKey);
                return await context.Products.ToListAsync();
            },
            cacheOptions)!;
        return products!;
    }
}