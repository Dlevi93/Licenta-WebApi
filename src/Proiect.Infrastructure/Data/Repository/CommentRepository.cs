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
    public class CommentRepository : ICommentRepository
    {
        private readonly AppDbContext _dbContext;

        public CommentRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Comment>> ListPaginatedAsync(int page, int pageSize, string filter, params Expression<Func<Comment, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Comment>();

            IQueryable<Comment> query = null;

            if (!string.IsNullOrEmpty(filter))
                query = result.Where(x => EF.Functions.Like(x.Text.ToLower(), $"%{filter.ToLower()}%"));

            query ??= result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.OrderByDescending(x => x.DateCreated);

            return await query.GetPagedAsync(page, pageSize);
        }

        public async Task<List<Comment>> ListLatestComments(params Expression<Func<Comment, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Comment>();

            IQueryable<Comment> query = result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.Where(x => x.DateCreated <= DateTime.UtcNow);

            query = query.OrderByDescending(x => x.DateCreated);

            query = query.Take(5);

            return await query.ToListAsync();
        }
    }
}
