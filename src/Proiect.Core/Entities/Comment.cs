using Proiect.Core.SharedKernel;

namespace Proiect.Core.Entities
{
    public class Comment : BaseEntity
    {
        public string FullName { get; set; }
        public string Email { get; set; }
        public string Text { get; set; }
        public string ImagePath { get; set; }
        
        public int ArticleId { get; set; }
        public virtual Article Article { get; set; }
    }
}
