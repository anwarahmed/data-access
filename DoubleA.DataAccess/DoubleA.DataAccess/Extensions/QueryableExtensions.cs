using System;
using System.Linq;
using System.Linq.Expressions;

namespace DoubleA.DataAccess.Extensions
{
    // ToDo: Move them to a separate Extensions nuget package
    internal static class QueryableExtensions
    {
        public static IQueryable<T> Filter<T>(this IQueryable<T> queryable, params Expression<Func<T, bool>>[] filters)
        {
            if (queryable == null)
            {
                throw new ArgumentNullException(nameof(queryable));
            }

            var validFilters = filters.WithoutNulls().ToList();

            foreach (var filter in validFilters)
            {
                queryable = queryable.Where(filter);
            }

            return queryable;
        }
    }
}
