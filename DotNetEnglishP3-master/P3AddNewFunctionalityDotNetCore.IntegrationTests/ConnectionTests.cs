using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Net;

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
    //Arrange
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
            //Act
            var canConnect = await context.Database.CanConnectAsync();
            //Assert
            canConnect.Should().BeTrue(); // Vérifie que la connexion à la base est possible
        }
    }


    [Fact]
    public async Task GET_retrieves_Product_page()
    {
        // Arrange : Créer une instance de WebApplicationFactory pour simuler l'application
        await using var application = new WebApplicationFactory<Program>();

        // Créer un client HTTP à partir de l'application
        using var client = application.CreateClient();

        // Act : Envoyer une requête GET à l'endpoint "/Product"
        var response = await client.GetAsync("/Product");

        // Assert : Vérifier que le statut HTTP est OK
        response.StatusCode.Should().Be(HttpStatusCode.OK);
    }
}
