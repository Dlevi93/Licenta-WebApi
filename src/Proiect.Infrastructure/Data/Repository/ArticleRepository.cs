using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces.Repository;

namespace Proiect.Infrastructure.Data.Repository
{
    public class ArticleRepository : IArticleRepository
    {
        private readonly AppDbContext _dbContext;

        public ArticleRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Article>> ListPaginatedAsync(int page, int pageSize, string filter, int categoryId, bool justActives, params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = null;

            if (categoryId != 0)
                query = result.Where(x => x.CategoryId == categoryId);

            query ??= result;

            if (justActives)
                query = query.Where(x => x.Active);

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(x => EF.Functions.Like(x.Text.ToLower(), $"%{filter.ToLower()}%") || x.Title.ToLower().Contains(filter.ToLower()));


            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Include(x => x.Category);

            return await query.GetPagedAsync(page, pageSize);
        }

        public async Task<List<Article>> ListNewArticles(params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active);

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(7);

            return await query.ToListAsync();
        }

        public async Task<List<Article>> ListLeadingArticles(params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active && x.Leading);

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(5);

            return await query.ToListAsync();
        }

        public async Task<List<Article>> ListArticlesByCategory(string category, params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = null;

            if (!string.IsNullOrEmpty(category))
                query = result.Where(x => EF.Functions.ILike(x.Category.NameSearch, $"%{category}%"));

            query ??= result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active);

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(5);

            return await query.ToListAsync();
        }

        public async Task<List<Article>> ListMostViewedArticlesByDays(int timeInDays, params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active && x.DateCreated > DateTime.UtcNow.AddDays(-timeInDays));

            query = query.OrderByDescending(x => x.Views);

            query = query.Take(5);

            return await query.ToListAsync();
        }

        public async Task<List<Article>> ListNewsAndActualitiesArticles(params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active && (x.Category.Name == "Hírek" || x.Category.Name == "Aktualitások"));

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(4);

            return await query.ToListAsync();
        }

        public async Task<Article> ListLatestPaper(params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = null;

            query = result.Where(x => EF.Functions.Like(x.Category.Name.ToLower(), "archívum"));

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow && x.Active);

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(1);

            return await query.FirstOrDefaultAsync();
        }

        public async Task<PagedResult<Article>> SearchArticles(int page, string categoryName, string text, params Expression<Func<Article, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Article>();

            IQueryable<Article> query = null;
            
            if (!string.IsNullOrEmpty(text))
                query = result.Where(x => EF.Functions.Like(x.Text.ToLower(), $"%{text.ToLower()}%") || x.Title.ToLower().Contains(text.ToLower()) || x.Author.ToLower().Contains(text.ToLower()));

            query ??= result;

            if (!string.IsNullOrEmpty(categoryName))
                query = result.Where(x=>string.Equals(x.Category.NameSearch.ToLower(), categoryName.ToLower()));

            query ??= result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.OrderByDescending(x => x.DateCreated);

            return await query.GetPagedAsync(page, 10);
        }
    }
}
