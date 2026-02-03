using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Common;
using BackEnd.Application.Common.Extensions;
using BackEnd.Application.Common.SearchCriteria;
using BackEnd.Application.Interfaces.Repositories.ProductRepo;
using BackEnd.Domain.Entities;
using BackEnd.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.Persistence.Repositories.ProductRepo
{
    public class ProductReadRepository : ReadRepository<Product, ProductSearchCriteria, int>, IProductReadRepository
    {
        public ProductReadRepository(ApplicationDbContext context) : base(context)
        {
        }
        public override IQueryable<Product> ApplyCustomFilters(
            IQueryable<Product> query,
            ProductSearchCriteria criteria)
        {
            var search = criteria.Search?.Trim();
            var name = criteria.Name?.Trim();

            query = query
                .WhereIf(!string.IsNullOrWhiteSpace(search),
                    p => EF.Functions.Like(p.Name, $"%{search}%") ||
                         EF.Functions.Like(p.Description, $"%{search}%"))

                .WhereIf(!string.IsNullOrWhiteSpace(name),
                    p => EF.Functions.Like(p.Name, $"%{name}%") ||
                         EF.Functions.Like(p.Description, $"%{name}%"))

                .WhereIf(criteria.MinPrice.HasValue,
                    p => p.Price >= criteria.MinPrice!.Value)

                .WhereIf(criteria.MaxPrice.HasValue,
                    p => p.Price <= criteria.MaxPrice!.Value)

                .WhereIf(!string.IsNullOrWhiteSpace(criteria.UserId),
                    p => p.UserId == criteria.UserId)

                .WhereIf(criteria.OnlyActive == true,
                    p => p.IsActive);

            return query;
        }

    }
}
