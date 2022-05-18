using System;

namespace Proiect.Web.ApiModels
{
    public class NewArticlesResponseDTO
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public int Views { get; set; }
        public DateTime DateCreated { get; set; }
        public CategoryResponseDTO Category { get; set; }
    }
}
