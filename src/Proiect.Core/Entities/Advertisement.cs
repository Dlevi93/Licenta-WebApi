using System.Collections.Generic;
using Proiect.Core.Common;
using Proiect.Core.SharedKernel;

namespace Proiect.Core.Entities
{
    public class Advertisement : BaseEntity
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Enums.AdPosition Position { get; set; }
        public virtual List<File> Files { get; set; }
    }
}
