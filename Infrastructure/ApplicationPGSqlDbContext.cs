using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure;
public  class ApplicationPGSqlDbContext:DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public ApplicationPGSqlDbContext(DbContextOptions<ApplicationPGSqlDbContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
