using System.Collections.Generic;
using Proiect.Core.SharedKernel;

namespace Proiect.Core.Entities
{
    public class Category : BaseEntity
    {
        public string Name { get; set; }
        public string NameSearch { get; set; }
        public int Order { get; set; }

        public int? ParentId { get; set; }

        public virtual List<Article> Articles { get; set; }
    }
}
