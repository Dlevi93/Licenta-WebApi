using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;
using Proiect.Core.Interfaces.Service;
using Proiect.Web.ApiModels;
using Proiect.Web.Helpers;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class ArticleController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly IArticleService _articleService;

        public ArticleController(IArticleService articleService, IRepository repository)
        {
            _articleService = articleService;
            _repository = repository;
        }

        [HttpGet]
        public async Task<IActionResult> GetPaginated(string sort, string order, int page, int pageSize, int categoryId, bool justActives, string filter)
        {
            var articles = await _articleService.GetPaginatedArticlesAsync(sort, order, page, pageSize, filter, justActives, categoryId);
            return new JsonResult(articles);
        }

        [HttpGet]
        [Route("{slug}")]
        public async Task<IActionResult> GetArticleBySlug([FromRoute] string slug)
        {
            var article = await _repository.WhereAsync<Article>(x => x.Slug.Equals(slug), x => x.Category, x => x.Files);
            return new JsonResult(article.SingleOrDefault());
        }

        [HttpPost]
        public async Task<IActionResult> AddArticle([FromBody] ArticleRequestDTO articleModel)
        {
            var article = new Article
            {
                CategoryId = articleModel.CategoryId,
                Text = articleModel.Text,
                Active = articleModel.Active,
                Leading = articleModel.Leading,
                Style = articleModel.Style,
                Title = articleModel.Title,
                DateCreated = articleModel.DateCreated,
                Author = articleModel.Author,
                Slug = $"{DateTime.UtcNow.Minute}-{DateTime.UtcNow.Second}-{new RandomGenerator().GenerateSlug(articleModel.Title)}"
            };

            var result = await _repository.AddAsync(article);
            var entityResult = await _repository.GetByIdAsync<Article>(result.Id, x => x.Category, x => x.Files);
            return new JsonResult(entityResult);
        }

        [HttpPut]
        [Route("{slug}")]
        public async Task<IActionResult> UpdateArticle([FromRoute] string slug, [FromBody] ArticleRequestDTO articleModel)
        {
            var article = await _repository.GetByIdAsync<Article>(articleModel.Id);

            article.CategoryId = articleModel.CategoryId;
            article.Text = articleModel.Text;
            article.Active = articleModel.Active;
            article.Leading = articleModel.Leading;
            article.Style = articleModel.Style;
            article.Title = articleModel.Title;
            article.DateUpdated = DateTime.UtcNow;
            article.Author = articleModel.Author;

            await _repository.UpdateAsync(article);

            var entityResult = await _repository.GetByIdAsync<Article>(articleModel.Id, x => x.Category, x => x.Files);
            return new JsonResult(entityResult);
        }

        [HttpPatch("uploadFile/{id}")]
        public async Task<IActionResult> UploadFile(string id, [FromForm(Name = "formData")] IFormFile file, [FromForm(Name = "author")] string author = "BÚSz")
        {
            if (file == null || file.Length == 0)
                return BadRequest();

            if (!file.IsValid()) return BadRequest("Extensii valide: *.png, *.jpg, *.pdf!");
            var (isSuccessful, isImage, dbPath) = await file.GenerateAndSave(id);

            if (!isSuccessful) return BadRequest();

            var articles = await _repository.WhereAsync<Article>(x => x.Slug.Equals(id), x => x.Category, x => x.Files);
            var article = articles.SingleOrDefault();

            var fileModel = new Core.Entities.File
            {
                ArticleId = article.Id,
                Author = author,
                FileName = file.FileName,
                Content = dbPath,
                ContentType = Path.GetExtension(file.FileName),
                FileType = isImage ? Enums.FileType.Image : Enums.FileType.Pdf
            };

            var result = await _repository.AddAsync(fileModel);
            return Ok(result);
        }

        [HttpDelete("removePic/{id:int}")]
        public async Task<IActionResult> DeletePic(int id)
        {
            var articleFile = await _repository.GetByIdAsync<Proiect.Core.Entities.File>(id);
            await _repository.DeleteAsync(articleFile);

            if (articleFile.Content.Contains(".jpg") || articleFile.Content.Contains(".png") || articleFile.Content.Contains(".gif"))
            {
                var extension = articleFile.Content.Substring(articleFile.Content.Length - 4);
                var path = articleFile.Content.Substring(0, articleFile.Content.Length - 4);
                var pathL = $"{path}-L{extension}";
                var pathM = $"{path}-M{extension}";
                var pathS = $"{path}-S{extension}";

                if (System.IO.File.Exists(pathL)) System.IO.File.Delete(pathL);
                if (System.IO.File.Exists(pathM)) System.IO.File.Delete(pathM);
                if (System.IO.File.Exists(pathS)) System.IO.File.Delete(pathS);
            }
            else if (articleFile.Content.Contains(".pdf"))
            {
                if (System.IO.File.Exists(articleFile.Content)) System.IO.File.Delete(articleFile.Content);
            }

            return Ok();
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteArticle(int id)
        {
            var result = await _repository.GetByIdAsync<Article>(id, x => x.Files);
            if (result == null) return NotFound();

            if (result.Files.Any())
            {
                foreach (var articleFile in result.Files.Where(articleFile => System.IO.File.Exists(articleFile.Content)))
                {
                    if (!articleFile.Content.Contains(".jpg") && !articleFile.Content.Contains(".png")) continue;

                    var path = articleFile.Content.Substring(0, articleFile.Content.Length - 4);
                    var pathL = $"{path}-L.jpg";
                    var pathM = $"{path}-M.jpg";
                    var pathS = $"{path}-S.jpg";

                    if (System.IO.File.Exists(pathL)) System.IO.File.Delete(pathL);
                    if (System.IO.File.Exists(pathM)) System.IO.File.Delete(pathM);
                    if (System.IO.File.Exists(pathS)) System.IO.File.Delete(pathS);
                }
            }

            await _repository.DeleteAsync(result);

            return Ok();
        }
    }

}
