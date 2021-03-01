using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Microsoft.EntityFrameworkCore;

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
        public IQueryable<TEntity> Tracked<TEntity>() where TEntity : class => Set<TEntity>();

        /// <inheritdoc/>
        public IQueryable<TEntity> Untracked<TEntity>() where TEntity : class => Set<TEntity>().AsNoTracking();

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
    }
}
