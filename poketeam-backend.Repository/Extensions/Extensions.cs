using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace poketeam_backend.Repository.Extensions
{
    public static class Extensions
    {
        // Used to include data from foreign keys: When model refers to a separate model
        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes)
            where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query, (current, include) => current.Include(include));
            }

            return query;
        }
    }
}
