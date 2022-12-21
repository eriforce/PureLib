using System.Collections.Generic;
using System.Linq;

namespace PureLib.Common {
    public sealed class PagedList<T> : List<T> {
        public int PageSize { get; private set; }
        public int Page { get; private set; }
        public int TotalCount { get; private set; }
        public int StartIndex {
            get { return (Page - 1) * PageSize; }
        }
        public int TotalPage {
            get {
                if ((TotalCount % PageSize) > 0)
                    return (TotalCount / PageSize) + 1;
                else
                    return TotalCount / PageSize;
            }
        }

        public PagedList(IEnumerable<T> pagedList, int page, int pageSize, int totalCount)
            : base(pagedList) {
            Page = page;
            PageSize = pageSize;
            TotalCount = totalCount;
        }

        public PagedList<TResult> Cast<TResult>() where TResult : class {
            return new PagedList<TResult>(this.Select(p => p as TResult), Page, PageSize, TotalCount);
        }
    }

    public static class PagedListExtensions {
        public static PagedList<T> AttachPagination<T>(this IEnumerable<T> data, int page, int pageSize, int totalCount) {
            return new PagedList<T>(data, page, pageSize, totalCount);
        }
    }
}