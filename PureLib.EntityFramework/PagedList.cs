using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using PureLib.Common;

namespace PureLib.EntityFramework {
    public static class PagedListExtension {
        public static PagedList<T> Paginate<T>(this IQueryable<T> query, int page, int pageSize) {
            int count = query.Count();
            return query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .AttachPagination(page, pageSize, count);
        }

        public static async Task<PagedList<T>> PaginateAsync<T>(this IQueryable<T> query, int page, int pageSize) {
            int count = await query.CountAsync();
            List<T> list = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
            return list.AttachPagination(page, pageSize, count);
        }
    }
}