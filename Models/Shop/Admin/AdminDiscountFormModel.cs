using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AspKnP231.Models.Shop.Admin
{
    public class AdminDiscountFormModel
    {
        [FromForm(Name = "discount-title")]
        [Required(ErrorMessage = "Вкажіть назву акції")]
        [StringLength(100, ErrorMessage = "Назва не повинна перевищувати 100 символів")]
        public String Title { get; set; } = null!;

        [FromForm(Name = "discount-description")]
        public String Description { get; set; } = String.Empty;

        [FromForm(Name = "discount-percent")]
        [Range(0, 100, ErrorMessage = "Відсоток має бути в межах від 0 до 100")]
        public double? Percent { get; set; }

        [FromForm(Name = "discount-price")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Акційна ціна має бути більшою за 0")]
        public double? Price { get; set; }

        [FromForm(Name = "discount-start")]
        [Required(ErrorMessage = "Вкажіть дату початку")]
        public DateTime Start { get; set; }

        [FromForm(Name = "discount-finish")]
        public DateTime? Finish { get; set; }
    }
}