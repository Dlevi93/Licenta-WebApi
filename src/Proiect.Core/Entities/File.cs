using Proiect.Core.Common;
using Proiect.Core.SharedKernel;

namespace Proiect.Core.Entities
{
    public class File : BaseEntity
    {
        public string FileName { get; set; }
        public string ContentType { get; set; }
        public string Content { get; set; }
        public Enums.FileType FileType { get; set; }

        public int? ArticleId { get; set; }
        public virtual Article Article { get; set; }

        public int? AdvertisementId { get; set; }
        public virtual Advertisement Advertisement { get; set; }
    }
}
