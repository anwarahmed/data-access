using System.Collections.Generic;
using System.Linq;

namespace DoubleA.DataAccess.Extensions
{
    // ToDo: Move them to a separate Extensions nuget package
    internal static class EnumerableExtensions
    {
        public static IEnumerable<TClass> WithoutNulls<TClass>(this IEnumerable<TClass> enumerable) where TClass : class
        {
            return enumerable == null ? Enumerable.Empty<TClass>() : enumerable.Where(c => c != null);
        }

        public static IEnumerable<TStruct> WithoutNulls<TStruct>(this IEnumerable<TStruct?> enumerable) where TStruct : struct
        {
            return enumerable == null ? Enumerable.Empty<TStruct>() : enumerable.Where(s => s.HasValue).Select(s => s.Value);
        }
    }
}
