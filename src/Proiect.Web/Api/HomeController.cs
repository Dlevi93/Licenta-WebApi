using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces;
using Proiect.Core.Interfaces.Repository;
using Proiect.Web.ApiModels;

namespace Proiect.Web.Api
{
    [Route("api/[controller]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IRepository _repository;
        private IArticleRepository _articleRepository;

        public HomeController(IRepository repository, IArticleRepository articleRepository)
        {
            _repository = repository;
            _articleRepository = articleRepository;
        }

        [HttpGet]
        [Route("leading")]
        public async Task<IActionResult> GetLeadingArticles()
        {
            var articles = await _articleRepository.ListLeadingArticles(x => x.Category, x => x.Files);
            return new JsonResult(articles.Select(x => new LeadingArticlesResponseDTO
            {
                Title = x.Title,
                Slug = x.Slug,
                Text = x.Files.Any() ? x.Text?.Length > 315 ? x.Text[..315] : x.Text
                        : x.Text?.Length > 500 ? x.Text[..500] : x.Text,
                DateCreated = x.DateCreated,
                Category = new CategoryResponseDTO { Name = x.Category.Name },
                Files = new List<FileResponseDTO>{ new()
                    {
                        Content = x.Files.FirstOrDefault(y => y.FileType == Enums.FileType.Image)?.Content

                    }
                }
            }));
        }

        [HttpGet]
        [Route("latest")]
        public async Task<IActionResult> GetLatestArticles()
        {
            var articles = await _articleRepository.ListNewArticles(x => x.Category);
            return new JsonResult(articles.Select(x => new NewArticlesResponseDTO
            {
                Title = x.Title,
                Slug = x.Slug,
                DateCreated = x.DateCreated,
                Category = new CategoryResponseDTO
                {
                    Name = x.Category.Name
                }
            }));
        }

        [HttpGet]
        [Route("byCategory/{category}")]
        public async Task<IActionResult> GetArticlesByCategory([FromRoute] string category)
        {
            var articles = await _articleRepository.ListArticlesByCategory(category, x => x.Category, x => x.Files);
            return new JsonResult(articles.Select(x => new LeadingArticlesResponseDTO
            {
                Title = x.Title,
                Author = x.Author,
                Slug = x.Slug,
                Text = x.Text?.Length > 250 ? x.Text[..250] : x.Text,
                DateCreated = x.DateCreated,
                Category = new CategoryResponseDTO
                {
                    Name = x.Category.Name
                },
                Files = new List<FileResponseDTO>{ new() { Content = x.Files.FirstOrDefault(y => y.FileType == Enums.FileType.Image)?.Content }
                }
            }));
        }

        [HttpGet]
        [Route("viewed/{timeInDays}")]
        public async Task<IActionResult> GetMostViewedArticlesByWeeks([FromRoute] int timeInDays)
        {
            var articles = await _articleRepository.ListMostViewedArticlesByDays(timeInDays, x => x.Category);
            return new JsonResult(articles.Select(x => new NewArticlesResponseDTO
            {
                Title = x.Title,
                Slug = x.Slug,
                Views = x.Views
            }));
        }

        [HttpGet]
        [Route("newsAndAct")]
        public async Task<IActionResult> GetNewsAndActualities()
        {
            var articles = await _articleRepository.ListNewsAndActualitiesArticles(x => x.Category, x => x.Files);
            return new JsonResult(articles.Select(x => new LeadingArticlesResponseDTO
            {
                Title = x.Title,
                Author = x.Author,
                Slug = x.Slug,
                Text = x.Text.Length > 250 ? x.Text[..250] : x.Text,
                DateCreated = x.DateCreated,
                Files = new List<FileResponseDTO>{ new() { Content = x.Files.FirstOrDefault(y => y.FileType == Enums.FileType.Image)?.Content }
                }
            }));
        }

        [HttpGet]
        [Route("getAllInterests")]
        public async Task<IActionResult> GetAllInterests()
        {
            var result = await _repository.ListAsync<Advertisement>(x => x.Files);
            return new JsonResult(result.Select(x => new Advertisement
            {
                Files = new List<File>{ new() { Content = x.Files.FirstOrDefault(y => y.FileType is Enums.FileType.Image or Enums.FileType.Gif)?.Content }
                },
                Link = x.Link,
                Name = x.Name,
                Position = x.Position
            }));
        }

        [HttpGet]
        [Route("article/{slug}")]
        public async Task<IActionResult> GetArticleBySlug([FromRoute] string slug)
        {
            var articles = await _repository.WhereAsync<Article>(x => x.Slug.Equals(slug), x => x.Category, x => x.Files, x => x.Comments);

            var article = articles.SingleOrDefault();

            if (article == null) return NoContent();

            article.Views++;
            await _repository.UpdateAsync(article);

            return new JsonResult(new ArticleResponseDTO
            {
                Category = new CategoryResponseDTO
                {
                    Name = article.Category.Name
                },
                Files = article.Files?.Select(x => new FileResponseDTO
                {
                    Content = x.Content,
                    ContentType = x.ContentType,
                    Author = x.Author
                }).ToList(),
                DateCreated = article.DateCreated,
                Text = article.Text,
                Slug = article.Slug,
                Author = article.Author,
                Style = article.Style,
                Title = article.Title,
                Comments = article.Comments?.Select(z => new CommentResponseDTO
                {
                    Id = z.Id,
                    FullName = z.FullName,
                    Email = z.Email,
                    Text = z.Text,
                    DateCreated = z.DateCreated,
                    ImagePath = z.ImagePath
                }).ToList()
            });
        }

        [HttpGet]
        [Route("article/meta/{slug}")]
        public async Task<IActionResult> GetArticleMetaBySlug([FromRoute] string slug)
        {
            var articles = await _repository.WhereAsync<Article>(x => x.Slug.Equals(slug), x => x.Files);

            var article = articles.SingleOrDefault();

            if (article == null) return NoContent();

            return new JsonResult(new ArticleResponseDTO
            {
                Files = article.Files?.Select(x => new FileResponseDTO
                {
                    Content = x.Content,
                    ContentType = x.ContentType,
                    Author = x.Author
                }).ToList(),
                Text = article.Text,
                Title = article.Title,
                Slug = article.Slug
            });
        }

        [HttpGet]
        [Route("article/related/{slug}")]
        public async Task<IActionResult> GetRelatedArticlesBySlug([FromRoute] string slug)
        {
            var articles = await _repository.WhereAsync<Article>(x => x.Slug.Equals(slug), x => x.Category, x => x.Files);

            var article = articles.SingleOrDefault();

            if (article == null) return NoContent();

            var related = await _articleRepository.ListArticlesByCategory(article.Category.NameSearch, x => x.Category, x => x.Files);
            return new JsonResult(related.Select(x => new LeadingArticlesResponseDTO
            {
                Title = x.Title,
                Author = x.Author,
                Slug = x.Slug,
                DateCreated = x.DateCreated,
                Files = new List<FileResponseDTO>{
                    new()
                    {
                        Content = x.Files.FirstOrDefault(y => y.FileType == Enums.FileType.Image)?.Content
                    }
                }
            }));
        }

        [HttpGet]
        [Route("latestPaper")]
        public async Task<IActionResult> GetLatestPaper()
        {
            var article = await _articleRepository.ListLatestPaper(x => x.Category, x => x.Files);
            if (article == null) return new JsonResult(new LeadingArticlesResponseDTO());
            return new JsonResult(new LeadingArticlesResponseDTO
            {
                Title = article.Title,
                Slug = article.Slug,
                DateCreated = article.DateCreated
            });
        }

        [HttpGet]
        [Route("search")]
        public async Task<IActionResult> SearchArticles(int page, string categoryName, string text)
        {
            var articles = await _articleRepository.SearchArticles(page, categoryName, text, x => x.Category, x => x.Files);
            articles.Results = articles.Results.Select(x => new Article
            {
                Title = x.Title,
                Author = x.Author,
                Slug = x.Slug,
                Text = x.Text?.Length > 500 ? x.Text[..500] : x.Text,
                DateCreated = x.DateCreated,
                Files = new List<File>
                {
                    new()
                    {
                        Content = x.Files.FirstOrDefault(y => y.FileType == Enums.FileType.Image)?.Content,
                        ContentType = ".jpg"
                    }
                },
                Category = new Category
                {
                    Name = x.Category.Name
                }
            }).ToList();

            return new JsonResult(articles);
        }
    }

}
