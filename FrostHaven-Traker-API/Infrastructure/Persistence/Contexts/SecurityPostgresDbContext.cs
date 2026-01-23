using Domain.Entities.Roles;
using Domain.Entities.Users;
using Microsoft.EntityFrameworkCore;

namespace Infrastructure.Persistence.Contexts;

public class SecurityPostgresDbContext : DbContext
{
    public DbSet<User> Users => Set<User>();
    public DbSet<Role> Roles => Set<Role>();
    public DbSet<UserRole> UserRoles => Set<UserRole>();
    
    public SecurityPostgresDbContext(DbContextOptions<SecurityPostgresDbContext> options)
        : base(options)
    {
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.HasDefaultSchema("security");
        modelBuilder.ApplyConfigurationsFromAssembly(typeof(SecurityPostgresDbContext).Assembly);
        base.OnModelCreating(modelBuilder);
    }
    
    
}