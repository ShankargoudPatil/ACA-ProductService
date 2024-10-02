using Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace Infrastructure;
public class ApplicationSqlServerSqlDbContext : DbContext
{
    public ApplicationSqlServerSqlDbContext(DbContextOptions<ApplicationSqlServerSqlDbContext> options)
        : base(options)
    {
    }

    public DbSet<Users> Users { get; set; }
    // Define DbSets for your entities   add-migration InitialSqlDb -Context ApplicationSqlServerSqlDbContext
}
