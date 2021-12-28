using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using DoubleA.DataAccess.Contract.Repositories;
using DoubleA.DataAccess.DataContexts;
using DoubleA.EntityModel.Entities;
using DoubleA.LinqExtensions;
using Microsoft.EntityFrameworkCore;

namespace DoubleA.DataAccess.Repositories
{
    public abstract class KeyedEntityRepository<TEntity> : IKeyedEntityRepository<TEntity> where TEntity : class, IKeyedEntity
    {
        public KeyedEntityRepository(IDataContext context)
        {
            Context = context;
        }

        protected IDataContext Context { get; }

        protected DbSet<TEntity> TrackedEntities => Context.Set<TEntity>();

        protected IQueryable<TEntity> UntrackedEntities => Context.Query<TEntity>();

        public virtual async Task<TEntity?> GetAsync(Guid id)
        {
            return await UntrackedEntities.FirstOrDefaultAsync(e => e.Id == id).ConfigureAwait(false);
        }

        public virtual async Task<IDictionary<Guid, TEntity>> GetAsync(ISet<Guid> ids)
        {
            return await UntrackedEntities.Where(e => ids.Contains(e.Id)).ToDictionaryAsync(e => e.Id).ConfigureAwait(false);
        }

        public virtual async Task<IDictionary<Guid, TEntity>> GetAsync()
        {
            return await UntrackedEntities.ToDictionaryAsync(e => e.Id).ConfigureAwait(false);
        }

        public virtual async Task<IDictionary<Guid, TEntity>> GetAsync(params Expression<Func<TEntity, bool>>[] filters)
        {
            return await UntrackedEntities.Filter(filters).ToDictionaryAsync(e => e.Id).ConfigureAwait(false);
        }

        public abstract Task<Guid> CreateAsync(TEntity newEntity);

        public abstract Task UpdateAsync(TEntity updatedEntity);

        public abstract Task RemoveAsync(Guid id);
    }
}
