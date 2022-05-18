using System;
using System.ComponentModel.DataAnnotations;
using Proiect.Core.Common;

namespace Proiect.Web.ApiModels
{
    public class ArticleRequestDTO
    {
        public int Id { get; set; }
        [Required]
        public string Author { get; set; }
        public DateTime DateCreated { get; set; }
        [Required]
        public string Title { get; set; }
        public string Text { get; set; }
        public bool Leading { get; set; }
        public bool Active { get; set; }
        public Enums.PageStyle Style { get; set; }
        [Required]
        public int CategoryId { get; set; }
    }
}
