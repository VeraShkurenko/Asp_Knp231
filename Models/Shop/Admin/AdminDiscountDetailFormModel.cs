using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AspKnP231.Models.Shop.Admin
{
    public class AdminDiscountDetailFormModel
    {
        [FromForm(Name = "discount-detail-discount-id")]
        [Required(ErrorMessage = "Оберіть акцію")]
        public String DiscountId { get; set; } = null!;

        [FromForm(Name = "discount-detail-product-id")]
        [Required(ErrorMessage = "Оберіть товар")]
        public String ProductId { get; set; } = null!;

        [FromForm(Name = "discount-detail-price")]
        [Required(ErrorMessage = "Вкажіть акційну ціну")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Ціна має бути більшою за 0")]
        public double? Price { get; set; }
    }
}