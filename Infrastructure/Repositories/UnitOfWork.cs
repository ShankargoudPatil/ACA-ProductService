using Domain.Entities;
using Domain.Persistence;
using Microsoft.EntityFrameworkCore.Storage;

namespace Infrastructure.Repositories;
public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationPGSqlDbContext _context;
    public IRepository<Product> Products { get; private set; }

    public UnitOfWork(ApplicationPGSqlDbContext context)
    {
        _context = context;
        Products = new Repository<Product>(_context);
    }

    public async Task<IDbContextTransaction> BeginTransactionAsync()
    {
        return await _context.Database.BeginTransactionAsync();
    }

    public async Task<int> CompleteAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
