using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using Xunit;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Product> Products { get; set; }
}

public class DatabaseIntegrationTests
{
    private readonly string _connectionString;

    public DatabaseIntegrationTests()
    {
        // Chaîne de connexion à la base SQL Server
        _connectionString = "Server=ROMAINPC\\SQLEXPRESSROMAIN;Database=P3Referential-2f561d3b-493f-46fd-83c9-6e2643e7bd0a;Trusted_Connection=True;MultipleActiveResultSets=true";
    }

    private AppDbContext CreateContext()
    {
        return new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseSqlServer(_connectionString)
                .Options);
    }

    [Fact]
    public async Task GET_retrieves_SQL_Server()
    {
        // Vérifier la connexion à la base de données via le DbContext
        using (var context = CreateContext())
        {
            var canConnect = await context.Database.CanConnectAsync();
            canConnect.Should().BeTrue(); // Vérifie que la connexion à la base est possible
        }

    }
}
