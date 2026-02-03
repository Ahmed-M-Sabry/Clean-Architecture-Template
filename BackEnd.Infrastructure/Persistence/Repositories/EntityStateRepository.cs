using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Abstractions.Persistence.BackEnd.Application.Abstractions.Persistence;
using BackEnd.Application.Common.SearchCriteria;
using BackEnd.Domain.Common;
using BackEnd.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class EntityStateRepository<TEntity, TId>
        : IEntityStateRepository<TEntity, TId>
        where TEntity : BaseEntity<TId>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _set;

        public EntityStateRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        public async Task ActivateAsync(TId id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null) return;

            entity.IsActive = true;
            entity.UpdatedOn = DateTime.UtcNow;
        }

        public async Task DeactivateAsync(TId id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null) return;

            entity.IsActive = false;
            entity.UpdatedOn = DateTime.UtcNow;
        }

        public async Task SoftDeleteAsync(TId id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null) return;

            entity.IsDeleted = true;
            entity.UpdatedOn = DateTime.UtcNow;
        }

        public async Task RestoreAsync(TId id)
        {
            var entity = await _set.FindAsync(id);
            if (entity == null) return;

            entity.IsDeleted = false;
            entity.UpdatedOn = DateTime.UtcNow;
        }

        public async Task<int> CountAsync(BaseSearchCriteria criteria)
        {
            IQueryable<TEntity> query = _set.AsQueryable();

            if (criteria.IsDeleted.HasValue)
                query = query.Where(e => e.IsDeleted == criteria.IsDeleted);

            if (criteria.IsActive.HasValue)
                query = query.Where(e => e.IsActive == criteria.IsActive);

            query = ApplyCountCustomFilters(query, criteria);

            return await query.CountAsync();
        }
        public virtual IQueryable<TEntity> ApplyCountCustomFilters(
           IQueryable<TEntity> query,
           BaseSearchCriteria criteria)
        {
            return query;
        }
    }
}
