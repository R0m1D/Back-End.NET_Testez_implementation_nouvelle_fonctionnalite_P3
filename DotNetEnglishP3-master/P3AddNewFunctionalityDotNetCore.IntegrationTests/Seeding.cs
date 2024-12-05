using System.Net;
using System.Threading.Tasks;
using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration; // Nécessaire pour accéder à IConfiguration
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
        // Configuration de l'instance pour charger le fichier appsettings.json
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory()) // Définir le chemin de base
            .AddJsonFile("appsettings.json") // Ajouter le fichier appsettings.json
            .Build();

        // Récupérer la chaîne de connexion depuis appsettings.json
        _connectionString = configuration.GetConnectionString("P3Referential");
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
