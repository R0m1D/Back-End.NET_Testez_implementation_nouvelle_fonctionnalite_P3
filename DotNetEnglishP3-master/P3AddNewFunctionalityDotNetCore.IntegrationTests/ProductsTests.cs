using FluentAssertions;
using Microsoft.EntityFrameworkCore;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.Entities;

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
    public async Task AddProductToCart_ShouldAddProductAndVerifyCartContents()
    {
        // Arrange: Création du contexte InMemory et initialisation
        using var context = CreateInMemoryContext();
        var cart = new Cart();
        var productEntity = new Product
        {
            Id = 1, // Assurez-vous qu'un identifiant est défini
            Name = "Sample Product",
            Price = 49.99,
            Quantity = 10,
            Description = "A high-quality sample product",
            Details = "This product is perfect for testing purposes."
        };

        // Ajout du produit à la base de données
        context.Products.Add(productEntity);
        await context.SaveChangesAsync();

        // Act: Ajouter le produit au panier
        cart.AddItem(productEntity, 2);

        // Assert: Vérifier que le produit est dans le panier
        var cartItems = cart.Lines.ToList(); // Récupération des lignes du panier
        cartItems.Should().HaveCount(1); // Vérifier qu'il y a un seul produit dans le panier
        var cartProduct = cartItems.First();
        cartProduct.Product.Name.Should().Be("Sample Product");
        cartProduct.Product.Price.Should().Be(49.99);
        cartProduct.Quantity.Should().Be(2);

        // Vérifier que les quantités en stock dans la base restent inchangées (car le panier est distinct)
        var addedProduct = await context.Products.FirstOrDefaultAsync(p => p.Id == productEntity.Id);
        addedProduct.Should().NotBeNull();
        addedProduct.Quantity.Should().Be(10); // La quantité d'origine reste inchangée
        //delete product
        context.Products.Remove(addedProduct);
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
