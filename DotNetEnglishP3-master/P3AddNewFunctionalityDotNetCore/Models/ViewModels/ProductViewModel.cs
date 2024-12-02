using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.ComponentModel.DataAnnotations;

namespace P3AddNewFunctionalityDotNetCore.Models.ViewModels
{
    public class ProductViewModel
    {
        // L'ID du produit ne doit pas être lié lors des soumissions de formulaire
        [BindNever]
        public int Id { get; set; }

        // Le nom du produit est obligatoire
        [Required(ErrorMessage = "MissingName")]
        public string Name { get; set; }

        // Le prix est obligatoire et doit être un nombre positif
        [Required(ErrorMessage = "MissingPrice")]
        //[Range(0.01, double.MaxValue, ErrorMessage = "PriceNotGreaterThanZero")]
        public string Price { get; set; }

        // La quantité en stock est obligatoire et doit être un entier positif
        [Required(ErrorMessage = "MissingQuantity")]
        [Range(1, int.MaxValue, ErrorMessage = "StockNotGreaterThanZero")]
        public string Stock { get; set; }

        // Description facultative
        public string Description { get; set; }

        // Détails facultatifs
        public string Details { get; set; }
    }
}
