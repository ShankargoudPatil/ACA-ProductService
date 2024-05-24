using Domain.Entities;
using Microsoft.EntityFrameworkCore.Storage;

namespace Domain.Persistence;
public interface IUnitOfWork
{
    IRepository<Product> Products { get; }
    Task<int> CompleteAsync();
    Task<IDbContextTransaction> BeginTransactionAsync();
}
