using BackEnd.Application.Common.SearchCriteria;
using BackEnd.Domain.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Application.Abstractions.Persistence
{
    public interface IReadRepository<TEntity, TEntitySC, TId>
        where TEntity : BaseEntity<TId>
        where TEntitySC : BaseSearchCriteria
    {
        IQueryable<TEntity> GetAllAsync();
        Task<TEntity?> GetByIdAsync(TId id);
        IQueryable<TEntity> GetAllBySearchCriteria(
            IQueryable<TEntity> query,
            TEntitySC criteria);
        IQueryable<TEntity> ApplyCustomFilters(
                IQueryable<TEntity> query,
                TEntitySC criteria);
    }
}
