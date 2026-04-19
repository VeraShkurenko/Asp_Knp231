using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace AspKnP231.Models.Home
{
    public class HomeFormsFormModel
    {
        [FromForm(Name = "user-name")]
        [Required(ErrorMessage = "Заповніть дане поле")]
        public String UserName { get; set; } = null!;

        
        [FromForm(Name = "user-email")]
        [Required(ErrorMessage = "Заповніть дане поле")]
        public String UserEmail { get; set; } = null!;        

        
        [FromForm(Name = "user-birthdate")]
        [Required(ErrorMessage = "Заповніть дане поле")]
        public DateTime? UserBirthdate { get; set; }

        
        [FromForm(Name = "user-login")]
        [Required(ErrorMessage = "Заповніть дане поле")]
        public String UserLogin { get; set; } = null!; 


        [FromForm(Name = "user-password")]
        [Required(ErrorMessage = "Заповніть дане поле")]
        public String UserPassword { get; set; } = null!;        

        [FromForm(Name = "user-repeat")]
        public String? UserRepeat { get; set; } = null!;

        [FromForm(Name = "user-avatar")]
        public IFormFile? UserAvatar { get; set; }

        [FromForm(Name = "user-button")]
        public String UserButton { get; set; } = null!;
    }
}
