using System;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Proiect.Core.Common;
using Proiect.Core.Entities;

namespace Proiect.Core.Interfaces.Repository
{
    public interface IAdvertisementRepository
    {
        Task<PagedResult<Advertisement>> ListPaginatedAsync(int page, int pageSize, string filter,
            params Expression<Func<Advertisement, object>>[] includeProperties);
    }
}
