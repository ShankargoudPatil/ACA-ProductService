using Domain.Entities;
namespace Domain.Persistence;
public interface IProductRepository : IRepository<Product>
{
    Task AddProduct(Product product,CancellationToken cancellationToken);
    Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken);
    Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken);
}
