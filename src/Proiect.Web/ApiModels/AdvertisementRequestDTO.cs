using Proiect.Core.Common;

namespace Proiect.Web.ApiModels
{
    public class AdvertisementRequestDTO
    {
        public string Name { get; set; }
        public string Link { get; set; }
        public Enums.AdPosition Position { get; set; }
    }
}
