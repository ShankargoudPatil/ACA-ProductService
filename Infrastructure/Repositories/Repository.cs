using System.Linq.Expressions;
using Domain.Persistence;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure.Repositories;
public class Repository<T> : IRepository<T> where T : class
{
    protected  ApplicationPGSqlDbContext _context;
    private readonly DbSet<T> _dbSet;
    public Repository(ApplicationPGSqlDbContext context)
    {
        _context = context;
        _dbSet = _context.Set<T>();
    }
    public async  Task AddAsync(T entity, CancellationToken cancellationToken)
    {
        await _dbSet.AddAsync(entity,cancellationToken);
    }

    public void Delete(T entity)
    {
        _dbSet.Remove(entity);
    }

    public async Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken)
    {
        return await _dbSet.ToListAsync(cancellationToken);
    }

    public async Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken)
    {
       return await _dbSet.AsNoTracking().FirstOrDefaultAsync(predicate,cancellationToken);
    }

    public async Task<T> GetByIdAsync(int id, CancellationToken cancellationToken)
    {
#pragma warning disable CS8603 // Possible null reference return.
        return await _dbSet.FindAsync(id,cancellationToken);
#pragma warning restore CS8603 // Possible null reference return.
    }

    public void Update(T entity)
    {
        _dbSet.Update(entity);
    }
}
