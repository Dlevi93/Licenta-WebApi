using System.ComponentModel.DataAnnotations;

namespace Proiect.Web.ApiModels
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "User Name is required")]
        public string Username { get; set; }

        [Required(ErrorMessage = "Password is required")]
        public string Password { get; set; }
        public bool RememberMe { get; set; }
    }
}
