using System;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Proiect.Core.Common;
using Proiect.Core.Entities;
using Proiect.Core.Interfaces.Repository;

namespace Proiect.Infrastructure.Data.Repository
{
    public class AdvertisementRepository : IAdvertisementRepository
    {
        private readonly AppDbContext _dbContext;

        public AdvertisementRepository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<PagedResult<Advertisement>> ListPaginatedAsync(int page, int pageSize, string filter, params Expression<Func<Advertisement, object>>[] includeProperties)
        {
            var result = _dbContext.Set<Advertisement>();

            IQueryable<Advertisement> query = null;

            if (!string.IsNullOrEmpty(filter))
                query = result.Where(x => EF.Functions.Like(x.Name.ToLower(), $"%{filter.ToLower()}%"));

            query ??= result;

            query = includeProperties.Aggregate(query, (current, property) => current.Include(property));

            query = query.OrderByDescending(x => x.DateCreated);

            return await query.GetPagedAsync(page, pageSize);
        }
    }
}
