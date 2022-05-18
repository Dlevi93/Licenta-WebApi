using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;
using Proiect.Core.Interfaces.Repository;
using Proiect.Web.ApiModels;
using Proiect.Web.Helpers;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AdvertisementController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IAdvertisementRepository _adRepository;

        public AdvertisementController(IRepository repository, IAdvertisementRepository adRepository)
        {
            _repository = repository;
            _adRepository = adRepository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginated(string sort, string order, int page, int pageSize, string filter)
        {
            var ads = await _adRepository.ListPaginatedAsync(page, pageSize, filter);
            return new JsonResult(ads);
        }

        [HttpPost]
        public async Task<IActionResult> AddAdvertisement([FromBody] AdvertisementRequestDTO adModel)
        {
            var advertisement = new Advertisement
            {
                Name = adModel.Name,
                Link = adModel.Link,
                Position = adModel.Position
            };

            var result = await _repository.AddAsync(advertisement);
            return new JsonResult(result);
        }

        [HttpPatch("uploadFile/{id}")]
        public async Task<IActionResult> UploadFile([FromRoute] int id, [FromForm(Name = "formData")] IFormFile file)
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (!file.IsValid()) return BadRequest("Extensii valide: *.png, *.jpg, *.gif!");
            var (isSuccessful, isImage, dbPath) = await file.GenerateAndSave(id.ToString(), true);

            if (!isSuccessful) return BadRequest();

            var ad = await _repository.GetByIdAsync<Advertisement>(id);

            var fileModel = new Proiect.Core.Entities.File
            {
                AdvertisementId = ad.Id,
                FileName = file.FileName,
                Content = dbPath,
                ContentType = Path.GetExtension(file.FileName),
                FileType = isImage ? Enums.FileType.Image : Enums.FileType.Gif
            };

            var result = await _repository.AddAsync(fileModel);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> RemoveAd([FromRoute] int id)
        {
            var result = await _repository.GetByIdAsync<Advertisement>(id, x => x.Files);

            foreach (var file in result.Files.Where(file => System.IO.File.Exists(file.Content)))
            {
                System.IO.File.Delete(file.Content);
            }

            if (result.Files.Any())
            {
                await _repository.DeleteRangeAsync(result.Files);
            }

            await _repository.DeleteAsync(result);
            
            return Ok();
        }
    }
}
