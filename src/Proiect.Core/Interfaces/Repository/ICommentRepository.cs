using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Proiect.Core.Common;
using Proiect.Core.Entities;

namespace Proiect.Core.Interfaces.Repository
{
    public interface ICommentRepository
    {
        Task<PagedResult<Comment>> ListPaginatedAsync(int page, int pageSize, string filter,
            params Expression<Func<Comment, object>>[] includeProperties);
        Task<List<Comment>> ListLatestComments(params Expression<Func<Comment, object>>[] includeProperties);
    }
}
