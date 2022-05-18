using System.Collections.Generic;
using Proiect.Core.Common;
using Proiect.Core.SharedKernel;

namespace Proiect.Core.Entities
{
    public class Article : BaseEntity
    {
        public string Title { get; set; }
        public string Slug { get; set; }
        public string Text { get; set; }
        public bool Leading { get; set; }
        public bool Active { get; set; }
        public int Views { get; set; }
        public Enums.PageStyle Style { get; set; }

        public virtual List<File> Files { get; set; }
        public virtual List<Comment> Comments { get; set; }

        public int CategoryId { get; set; }
        public virtual Category Category { get; set; }
    }
}
