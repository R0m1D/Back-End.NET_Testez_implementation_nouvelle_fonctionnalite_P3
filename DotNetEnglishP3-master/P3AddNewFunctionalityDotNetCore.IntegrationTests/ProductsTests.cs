using FluentAssertions;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System.Net;

public class InMemoryDatabaseIntegrationTests
{
    private AppDbContext CreateInMemoryContext()
    {
        return new AppDbContext(
            new DbContextOptionsBuilder<AppDbContext>()
                .UseInMemoryDatabase("TestDatabase")
                .Options);
    }

    [Fact]
    public async Task AddProduct_ToInMemoryDatabase_ShouldSucceed()
    {
        // Arrange: Création du contexte InMemory
        using var context = CreateInMemoryContext();

        var productEntity = new Product
        {
            Name = "Sample Product",
            Price = 49.99,
            Quantity = 10,
            Description = "A high-quality sample product",
            Details = "This product is perfect for testing purposes."
        };

        // Act: Ajout du produit
        context.Products.Add(productEntity);
        await context.SaveChangesAsync();

        // Assert: Vérification
        var addedProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Sample Product");
        addedProduct.Should().NotBeNull();
        addedProduct.Name.Should().Be("Sample Product");
        addedProduct.Price.Should().Be(49.99);
        addedProduct.Quantity.Should().Be(10);
        addedProduct.Description.Should().Be("A high-quality sample product");
        addedProduct.Details.Should().Be("This product is perfect for testing purposes.");

        //supprime le produit pour ne pas interférer avec le test de supression
        var productToDelete = await context.Products.FirstOrDefaultAsync(p => p.Name == "Sample Product");
    }

    [Fact]
    public async Task DeleteProduct_FromInMemoryDatabase_ShouldSucceed()
    {
        // Arrange: Création du contexte InMemory et ajout d'un produit
        using var context = CreateInMemoryContext();

        var productEntity = new Product
        {
            Name = "Test Product",
            Price = 4.99,
            Quantity = 42,
            Description = "sample product",
            Details = "for testing purposes."
        };

        context.Products.Add(productEntity);
        await context.SaveChangesAsync();

        // Act: Suppression du produit
        var productToDelete = await context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        productToDelete.Should().NotBeNull();

        context.Products.Remove(productToDelete);
        await context.SaveChangesAsync();

        // Assert: Vérification de la suppression
        var deletedProduct = await context.Products.FirstOrDefaultAsync(p => p.Name == "Test Product");
        deletedProduct.Should().BeNull(); // Le produit ne doit plus exister
    }
}
