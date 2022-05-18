using System.Threading.Tasks;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces.Repository;
using Proiect.Core.Interfaces.Service;

namespace Proiect.Core.Services
{
    public class ArticleService : IArticleService
    {
        private readonly IArticleRepository _articleRepository;

        public ArticleService(IArticleRepository articleRepository)
        {
            _articleRepository = articleRepository;
        }

        public async Task<PagedResult<Article>> GetPaginatedArticlesAsync(string sort, string order, int page, int pageSize, string filter, bool justActives, int categoryId)
        {
            var result = await _articleRepository.ListPaginatedAsync(page, pageSize, filter, categoryId, justActives, x => x.Category);
            return result;
        }
    }
}
