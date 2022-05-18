using System.ComponentModel.DataAnnotations;

namespace Proiect.Web.ApiModels
{
    public class NewsletterRequestDTO
    {
        [Required]
        [MaxLength(512)]
        public string Email { get; set; }
    }
}
