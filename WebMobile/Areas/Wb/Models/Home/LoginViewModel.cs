using System.ComponentModel.DataAnnotations;

namespace WebMobile.Areas.Wb.Models.Home
{
    public class LoginViewModel
    {
        [Required]
        [Display(Name = "Account")]
        public string UserName { get; set; }

        [Required]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        public string Password { get; set; }

        [Display(Name = "RememberMe?")]
        public bool IsRememberMe { get; set; }

        public string ReturnUrl { get; set; }
    }
}
