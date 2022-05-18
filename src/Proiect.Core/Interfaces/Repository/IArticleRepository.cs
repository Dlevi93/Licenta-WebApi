using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Proiect.Core.Common;
using Proiect.Core.Entities;

namespace Proiect.Core.Interfaces.Repository
{
    public interface IArticleRepository
    {
        Task<PagedResult<Article>> ListPaginatedAsync(int page, int pageSize, string filter, int categoryId, bool justActives,
            params Expression<Func<Article, object>>[] includeProperties);
        Task<List<Article>> ListNewArticles(params Expression<Func<Article, object>>[] includeProperties);
        Task<List<Article>> ListLeadingArticles(params Expression<Func<Article, object>>[] includeProperties);
        Task<List<Article>> ListArticlesByCategory(string category,
            params Expression<Func<Article, object>>[] includeProperties);
        Task<List<Article>> ListMostViewedArticlesByDays(int timeInDays, params Expression<Func<Article, object>>[] includeProperties);

        Task<List<Article>>
            ListNewsAndActualitiesArticles(params Expression<Func<Article, object>>[] includeProperties);

        Task<Article> ListLatestPaper(params Expression<Func<Article, object>>[] includeProperties);

        Task<PagedResult<Article>> SearchArticles(int page, string categoryName, string text,
            params Expression<Func<Article, object>>[] includeProperties);
    }
}
