using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Common.SearchCriteria
{
    public class ProductSearchCriteria : BaseSearchCriteria
    {
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? Name { get; set; }
        public string? UserId { get; set; }
        public bool? OnlyActive { get; set; } = true;
    }

}
