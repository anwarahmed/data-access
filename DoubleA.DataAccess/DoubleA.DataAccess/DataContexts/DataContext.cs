using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using DoubleA.EntityModel.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

namespace DoubleA.DataAccess.DataContexts
{
    public abstract class DataContext : DbContext, IDataContext
    {
        /// <summary>
        /// Initialize a new instance of the data context
        /// </summary>
        protected DataContext()
        {
        }

        /// <summary>
        /// Initialize a new instance of the data context with the given options
        /// </summary>
        /// <param name="options"></param>
        protected DataContext([NotNull] DbContextOptions options) : base(options)
        {
        }

        /// <inheritdoc/>
        public bool HasChanges => ChangeTracker.HasChanges();

        /// <inheritdoc/>
        public IQueryable<TEntity> Query<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

        /// <inheritdoc/>
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            UpdateTrackedEntitiesBeforeSaving();
            return await base.SaveChangesAsync(cancellationToken);
        }

        /// <inheritdoc/>
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            base.OnConfiguring(optionsBuilder);
        }

        /// <inheritdoc/>
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
        }

        /// <summary>
        /// Make changes to a newly created tracked entity before saving
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void OnAddingTrackedEntityBeforeSaving(EntityEntry<ITrackedEntity> entry)
        {
            entry.Property(entity => entity.CreatedOn).CurrentValue = DateTime.UtcNow;
            entry.Property(entity => entity.UpdatedOn).CurrentValue = DateTime.UtcNow;
        }

        /// <summary>
        /// Make changes to an updated tracked entity before saving
        /// </summary>
        /// <param name="entry"></param>
        protected virtual void OnModifyingTrackedEntityBeforeSaving(EntityEntry<ITrackedEntity> entry)
        {
            entry.Property(entity => entity.CreatedOn).IsModified = false;
            entry.Property(entity => entity.UpdatedOn).CurrentValue = DateTime.UtcNow;
        }

        private void UpdateTrackedEntitiesBeforeSaving()
        {
            var trackedEntities =
                ChangeTracker
                    .Entries<ITrackedEntity>()
                    .GroupBy(entity => entity.State)
                    .ToDictionary(group => group.Key, group => group.ToList());

            if (trackedEntities.TryGetValue(EntityState.Added, out var addedEntities))
            {
                addedEntities.ForEach(entry => OnAddingTrackedEntityBeforeSaving(entry));
            }

            if (trackedEntities.TryGetValue(EntityState.Modified, out var modifiedEntities))
            {
                modifiedEntities.ForEach(entry => OnModifyingTrackedEntityBeforeSaving(entry));
            }
        }
    }
}
