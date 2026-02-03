using BackEnd.Application.Abstractions.Persistence;
using BackEnd.Domain.Common;
using BackEnd.Infrastructure.Persistence.DbContext;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BackEnd.Infrastructure.Persistence.Repositories
{
    public class GenericRepository<TEntity, TId> : IGenericRepository<TEntity, TId>
    where TEntity : BaseEntity<TId>
    {
        protected readonly ApplicationDbContext _context;
        protected readonly DbSet<TEntity> _set;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _set = context.Set<TEntity>();
        }

        public virtual async Task<TEntity?> GetByIdAsync(TId id)
        {
            return await _set.FirstOrDefaultAsync(e => e.Id!.Equals(id));
        }

        public virtual async Task AddAsync(TEntity entity)
        {
            entity.CreatedOn = DateTime.UtcNow;
            await _set.AddAsync(entity);
        }

        public virtual void Update(TEntity entity)
        {
            entity.UpdatedOn = DateTime.UtcNow;
            _set.Update(entity);
        }

        public virtual void Remove(TEntity entity)
        {
            entity.IsDeleted = true;
            _set.Update(entity);
        }
    }
}
