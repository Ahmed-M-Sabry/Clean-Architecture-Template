using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Common.SearchCriteria
{
    public abstract class BaseSearchCriteria
    {
        public string? Search { get; set; }
        public bool? IsDeleted { get; set; } = false;

        public int? PageIndex { get; set; }
        public int? PageSize { get; set; } 

        public bool? IsActive { get; set; }

        public int MaxPageSize = 10000;
    }

}
