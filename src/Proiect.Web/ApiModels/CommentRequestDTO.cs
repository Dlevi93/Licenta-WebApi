using System.ComponentModel.DataAnnotations;

namespace Proiect.Web.ApiModels
{
    public class CommentRequestDTO
    {
        [Required]
        public string FullName { get; set; }
        [Required]
        public string Email { get; set; }
        [Required]
        [MaxLength(512)]
        public string Text { get; set; }
    }
}
