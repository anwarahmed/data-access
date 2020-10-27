using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace DoubleA.DataAccess.DataContexts
{
    public interface IDataContext
    {
        /// <summary>
        /// Returns true if context contains modified entities
        /// </summary>
        bool HasChanges { get; }

        /// <summary>
        /// Get the DbSet for a given entity that can be used to make changes to the underlying data collection
        /// </summary>
        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        /// <summary>
        /// Get a query for a given entity for viewing purposes only
        /// </summary>
        IQueryable<TEntity> Query<TEntity>() where TEntity : class;

        /// <summary>
        /// Save changes made to all modified entities in the current data context
        /// </summary>
        Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    }
}
