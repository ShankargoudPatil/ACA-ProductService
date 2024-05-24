using Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure;
public  class ApplicationContext:DbContext
{
    public DbSet<Product> Products { get; set; }
    public DbSet<Category> Categories { get; set; }
    public DbSet<Manufacturer> Manufacturers { get; set; }
    public ApplicationContext(DbContextOptions<ApplicationContext> options):base(options)
    {
        
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

    }
}
