using System.Linq.Expressions;
namespace Domain.Persistence;
public interface IRepository<T> where T : class
{
    Task<T> GetByIdAsync(int id, CancellationToken cancellationToken);
    Task<IEnumerable<T>> GetAllAsync(CancellationToken cancellationToken);
    Task AddAsync(T entity, CancellationToken cancellationToken);
    void Update(T entity);
    void Delete(T entity);
    Task<T> GetByConditionAsync(Expression<Func<T, bool>> predicate, CancellationToken cancellationToken);
}
