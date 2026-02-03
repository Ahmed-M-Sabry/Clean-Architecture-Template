using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Common.SearchCriteria;
using BackEnd.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Interfaces.Repositories.ProductRepo
{
    public interface IProductReadRepository : IReadRepository<Product, ProductSearchCriteria, int>
    {
    }
}
