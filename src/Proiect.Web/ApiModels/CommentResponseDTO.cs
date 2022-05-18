using System;

namespace Proiect.Web.ApiModels
{
    public class CommentResponseDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public DateTime DateCreated { get; set; }
        public string ImagePath { get; set; }
    }
}
