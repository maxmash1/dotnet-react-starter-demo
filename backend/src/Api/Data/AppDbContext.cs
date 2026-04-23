using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>
/// Entity Framework Core database context for the application.
/// Aggregates all <see cref="DbSet{TEntity}"/> entities used by the API.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of the <see cref="AppDbContext"/> class.
    /// </summary>
    /// <param name="options">EF Core configuration options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options)
    {
    }

    /// <summary>
    /// Persisted set of employee entities.
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <inheritdoc />
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Employee>(entityBuilder =>
        {
            entityBuilder.HasKey(employee => employee.EmployeeId);
            entityBuilder.Property(employee => employee.Name).IsRequired().HasMaxLength(100);
            entityBuilder.Property(employee => employee.Email).HasMaxLength(255);
            entityBuilder.Property(employee => employee.Department).HasMaxLength(50);
            entityBuilder.Property(employee => employee.Salary).HasPrecision(10, 2);
        });
    }
}
