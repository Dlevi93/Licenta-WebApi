using System;
using System.Collections.Generic;

namespace Proiect.Web.ApiModels
{
    public class LeadingArticlesResponseDTO
    {
        public string Title { get; set; }
        public string Author { get; set; }
        public string Slug { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public CategoryResponseDTO Category { get; set; }
        public List<FileResponseDTO> Files { get; set; }
    }
}
