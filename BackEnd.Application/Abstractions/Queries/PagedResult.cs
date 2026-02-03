using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Abstractions.Queries
{
    public sealed class PagedResult<T>
    {
        public PagedResult(IReadOnlyList<T> dtoList, int total, int pageIndex, int pageSize)
        {
            Items = dtoList;
            TotalRows = total;
            PageIndex = pageIndex;
            PageSize = pageSize;
        }

        public int TotalRows { get; init; }
        public int PageSize { get; init; }
        public int PageIndex { get; init; }
        public IReadOnlyList<T> Items { get; init; } = [];
    }
}
