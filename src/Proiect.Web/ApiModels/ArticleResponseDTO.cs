using System;
using System.Collections.Generic;
using Proiect.Core.Common;

namespace Proiect.Web.ApiModels
{
    public class ArticleResponseDTO
    {
        public CategoryResponseDTO Category { get; set; }
        public List<FileResponseDTO> Files { get; set; }
        public DateTime DateCreated { get; set; }
        public string Text { get; set; }
        public string Slug { get; set; }
        public string Author { get; set; }
        public Enums.PageStyle Style { get; set; }
        public string Title { get; set; }
        public List<CommentResponseDTO> Comments { get; set; }

    }
}
