using System.Threading.Tasks;
using Proiect.Core.Common;
using Proiect.Core.Entities;

namespace Proiect.Core.Interfaces.Service
{
    public interface IArticleService
    {
        Task<PagedResult<Article>> GetPaginatedArticlesAsync(string sort, string order, int page, int pageSize,
            string filter, bool justActives, int categoryId);
    }
}
