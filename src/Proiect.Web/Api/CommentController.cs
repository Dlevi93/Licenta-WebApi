using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;
using Proiect.Core.Interfaces.Repository;
using Proiect.Web.ApiModels;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class CommentController : ControllerBase
    {
        private readonly IRepository _repository;
        private readonly ICommentRepository _commentRepository;

        public CommentController(IRepository repository, ICommentRepository commentRepository)
        {
            _repository = repository;
            _commentRepository = commentRepository;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetPaginated(string sort, string order, int page, int pageSize, string filter)
        {
            var comments = await _commentRepository.ListPaginatedAsync(page, pageSize, filter, x => x.Article);
            comments.Results = comments.Results.Select(x => new Comment
            {
                Id = x.Id,
                FullName = x.FullName,
                Text = x.Text,
                DateCreated = x.DateCreated,
                Email = x.Email,
                ImagePath = x.ImagePath,
                Article = new Article
                {
                    Slug = x.Article.Slug,
                    Title = x.Article.Title
                }
            }).ToList();
            return new JsonResult(comments);
        }

        [HttpPost]
        [Route("{slug}")]
        public async Task<IActionResult> AddComment([FromRoute] string slug, [FromBody] CommentRequestDTO commentModel)
        {
            var articles = await _repository.WhereAsync<Article>(x => x.Slug.Equals(slug), x => x.Category, x => x.Files);
            var article = articles.SingleOrDefault();

            if (article == null) return BadRequest();

            var comment = new Comment
            {
                FullName = commentModel.FullName,
                Email = commentModel.Email,
                Text = commentModel.Text,
                ArticleId = article.Id
            };

            var result = await _repository.AddAsync(comment);
            return new JsonResult(result);
        }

        [HttpDelete("{id}")]
        [Authorize]
        public async Task<IActionResult> RemoveComment(int id)
        {
            var result = await _repository.GetByIdAsync<Comment>(id);
            await _repository.DeleteAsync(result);
            return Ok();
        }

        [HttpGet]
        [Route("latest")]
        public async Task<IActionResult> GetLatestComments()
        {
            var comments = await _commentRepository.ListLatestComments(x => x.Article.Files);
            return new JsonResult(comments.Select(x => new Comment
            {
                FullName = x.FullName,
                Text = x.Text,
                DateCreated = x.DateCreated,
                Article = new Article
                {
                    Slug = x.Article.Slug,
                    Title = x.Article.Title
                }
            }));
        }
    }
}
