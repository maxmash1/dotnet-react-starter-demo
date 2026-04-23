using Api.Domain;
using Microsoft.EntityFrameworkCore;

namespace Api.Data;

/// <summary>
/// Entity Framework Core database context for the Organization API.
/// Configured with an InMemory provider for development and testing.
/// </summary>
public class AppDbContext : DbContext
{
    /// <summary>
    /// Initializes a new instance of AppDbContext with the provided options.
    /// </summary>
    /// <param name="options">Database context configuration options.</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Employees dataset.
    /// </summary>
    public DbSet<Employee> Employees => Set<Employee>();

    /// <summary>
    /// Configures entity mappings and seeds development/test data.
    /// </summary>
    /// <param name="modelBuilder">The model builder used to configure entities.</param>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed data for in-memory development/testing
        modelBuilder.Entity<Employee>().HasData(
            new Employee
            {
                Id = 1,
                FullName = "Alice Johnson",
                Email = "alice.johnson@example.com",
                Department = "Engineering",
                JobTitle = "Senior Software Engineer",
                HireDate = new DateTime(2020, 3, 15),
                ActiveIndicator = true
            },
            new Employee
            {
                Id = 2,
                FullName = "Bob Martinez",
                Email = "bob.martinez@example.com",
                Department = "Engineering",
                JobTitle = "Software Engineer",
                HireDate = new DateTime(2021, 7, 1),
                ActiveIndicator = true
            },
            new Employee
            {
                Id = 3,
                FullName = "Carol White",
                Email = "carol.white@example.com",
                Department = "Product",
                JobTitle = "Product Manager",
                HireDate = new DateTime(2019, 11, 20),
                ActiveIndicator = true
            },
            new Employee
            {
                Id = 4,
                FullName = "David Lee",
                Email = "david.lee@example.com",
                Department = "Design",
                JobTitle = "UX Designer",
                HireDate = new DateTime(2022, 1, 10),
                ActiveIndicator = true
            },
            new Employee
            {
                Id = 5,
                FullName = "Eva Brown",
                Email = "eva.brown@example.com",
                Department = "HR",
                JobTitle = "HR Manager",
                HireDate = new DateTime(2018, 6, 5),
                ActiveIndicator = false
            }
        );
    }
}
