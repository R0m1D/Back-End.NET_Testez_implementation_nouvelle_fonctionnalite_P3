using Xunit;
using Moq;
using Microsoft.Extensions.Localization;
using P3AddNewFunctionalityDotNetCore.Models.Repositories;
using P3AddNewFunctionalityDotNetCore.Models.Services;
using P3AddNewFunctionalityDotNetCore.Models;
using P3AddNewFunctionalityDotNetCore.Models.ViewModels;
using P3AddNewFunctionalityDotNetCore.Models.Entities;
using System;


namespace P3AddNewFunctionalityDotNetCore.Tests
{
    public class ProductServiceTests
    {
        // Mock des dépendances
        private readonly Mock<ICart> _mockCart;
        private readonly Mock<IProductRepository> _mockProductRepository;
        private readonly Mock<IOrderRepository> _mockOrderRepository;
        private readonly Mock<IStringLocalizer<ProductService>> _mockLocalizer;

        // Instance du service à tester
        private readonly ProductService _productService;
        public ProductServiceTests()
        {
            // Initialisation des mocks
            _mockCart = new Mock<ICart>();
            _mockProductRepository = new Mock<IProductRepository>();
            _mockOrderRepository = new Mock<IOrderRepository>();
            _mockLocalizer = new Mock<IStringLocalizer<ProductService>>();

            // Instantiation du ProductService avec les dépendances mockées
            _productService = new ProductService(
                _mockCart.Object,
                _mockProductRepository.Object,
                _mockOrderRepository.Object,
                _mockLocalizer.Object
            );
        }
        [Fact]
        public void SaveProduct_CheckValideProduct()
        {
            // Arrange
            var productViewModel = new ProductViewModel
            {
                Name = "Test",
                Price = 813,
                Stock = 42,
                Description = "Description test",
                Details = "detail test"
            };
            // Act
            _productService.SaveProduct( productViewModel );

            // Assert
            _mockProductRepository.Verify(repo => repo.SaveProduct(It.IsAny<Product>()), Times.Once);
        }



        [Fact]
        public void SaveProduct_CheckNotValideProduct_ShouldNotCallFuncionToaddProduct()
        {
            // Arrange
            var noName = new ProductViewModel
            {
                Name = "",                              // Nom invalide
                Price = 813,                            // Prix valide
                Stock = 42,                             // Stock valide
                Description = "description inutile",    // Description remplie
                Details = "detail inutile"              // Détails remplie
            };
            var wrongPrice = new ProductViewModel
            {
                Name = "test",                          // Nom invalide
                Price = -13,                            // Prix invalide
                Stock = 42,                             // Stock valide
                Description = "description inutile",    // Description remplie
                Details = "detail inutile"              // Détails remplie
            };
            var wrongStock = new ProductViewModel
            {
                Name = "test",                          // Nom invalide
                Price = 813,                            // Prix valide
                Stock = -2,                             // Stock invalide
                Description = "description inutile",    // Description remplie
                Details = "detail inutile"              // Détails remplie
            };
            var noPrice = new ProductViewModel
            {
                Name = "test",                          // Nom valide                       
                                                        // Prix invalide
                Stock = 42,                             // Stock valide
                Description = "description inutile",    // Description remplie
                Details = "detail inutile"              // Détails remplie
            };
            var noStock = new ProductViewModel
            {
                Name = "test",                          // Nom valide
                Price = 813,                            // Prix valide
                                                        // Stock invalide
                Description = "description inutile",    // Description remplie
                Details = "detail inutile"              // Détails remplie
            };

            // Act - appel de la méthode SaveProduct qui devrait inclure une validation
            Action act = () => _productService.SaveProduct(noName);
            Action act2 = () => _productService.SaveProduct(wrongPrice);
            Action act3 = () => _productService.SaveProduct(wrongStock);
            Action act4 = () => _productService.SaveProduct(noPrice);
            Action act5 = () => _productService.SaveProduct(noStock);


            // Assert - vérifier que la méthode SaveProduct du repository n'a pas été appelée
            _mockProductRepository.Verify(repo => repo.SaveProduct(It.IsAny<Product>()), Times.Never); // Vérifie que la méthode n'a pas été appelée
        }



        [Fact]
        public void DeleteProduct_ProductExists_ShouldCallRemoveLineAndDeleteProduct()
        {
            // Arrange
            var productId = 1;
            var mockCart = new Mock<ICart>();
            var mockProductRepository = new Mock<IProductRepository>();
            var mockProductService = new Mock<IProductService>();

            var productService = new ProductService(
                mockCart.Object,
                mockProductRepository.Object,
                null,
                null  
            );

            var product = new Product { Id = productId, Name = "Test Product" };

            // Setup: Mock the GetProductById method to return the correct product
            mockProductService.Setup(service => service.GetProductById(productId)).Returns(product);

            // Act: Call the DeleteProduct method
            productService.DeleteProduct(productId);

            // Assert: Check that RemoveLine and DeleteProduct have been called
            mockCart.Verify(cart => cart.RemoveLine(It.IsAny<Product>()), Times.Once);
            mockProductRepository.Verify(repo => repo.DeleteProduct(productId), Times.Once);
        }

    }
}