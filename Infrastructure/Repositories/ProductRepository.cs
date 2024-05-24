using Domain.Entities;
using Domain.Persistence;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Repositories;

public class ProductRepository : Repository<Product>, IProductRepository
{
    private readonly DbSet<Product> _dbSet;
    public ProductRepository(ApplicationContext context) : base(context)
    {
        _dbSet = context.Set<Product>();
    }
    public async Task AddProduct(Product product, CancellationToken cancellationToken)
    {
         await  _dbSet.AddAsync(product,cancellationToken);
    }

    public async Task<Product> GetProductByIdAsync(Guid id, CancellationToken cancellationToken)
    {
        return await _dbSet.FindAsync(id,cancellationToken);
    }

    public async Task<IEnumerable<Product>> GetProductsAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }
}
