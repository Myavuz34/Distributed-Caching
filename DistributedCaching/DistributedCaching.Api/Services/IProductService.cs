using DistributedCaching.Api.Models;

namespace DistributedCaching.Api.Services;

public interface IProductService
{
    Task<Product> GetAsync(int id);
    Task<List<Product>> GetAllAsync();
    Task AddSync(ProductCreationDto product);
}