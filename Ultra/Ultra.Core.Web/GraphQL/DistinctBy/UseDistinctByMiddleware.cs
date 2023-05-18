using DelegateDecompiler;
using HotChocolate.Resolvers;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Core.Web.GraphQL.DistinctBy
{
    [Obsolete("Не работает: The given key 'EmptyProjectionMember' was not present in the dictionary")]
    public class UseDistinctByMiddleware<TEntity>
    {
        private FieldDelegate _next;

        public UseDistinctByMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            await _next.Invoke(context).ConfigureAwait(false);

            var fieldName = context.ArgumentValue<string?>(UseDistinctByConstants.ArgumentName);
            if (string.IsNullOrEmpty(fieldName))
            {
                return;
            }

            if (context.Result is not IQueryable query)
            {
                return;
            }

            context.Result = ApplyDistinct(query, fieldName);
        }

        private IQueryable ApplyDistinct(IQueryable query, string fieldName)
        {
            /*
                query
                    .GroupBy(x => x.[fieldName])
                    .Select(g => g.First())
             */

            var entityType = typeof(TEntity);
            var property = entityType.GetProperty(fieldName);

            var paramExpr = Expression.Parameter(entityType, "x");
            var propAccessExpr = Expression.Property(paramExpr, property);
            var lambdaExpr = Expression.Lambda(propAccessExpr, paramExpr);

            var groupByMethod = typeof(Queryable).GetMethods()
                .First(m =>
                    m.Name == "GroupBy" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 2);
            var groupByGenericMethod = groupByMethod.MakeGenericMethod(entityType, property.PropertyType);

            var groups = groupByGenericMethod.Invoke(null, new object[] { query, lambdaExpr });
            var unwrapMethod = GetType()
               .GetMethod(nameof(UnWrapGroups), BindingFlags.Static | BindingFlags.NonPublic)
               .MakeGenericMethod(new[] { entityType, property.PropertyType });

            return (IQueryable<TEntity>)unwrapMethod.Invoke(null, new object[] { groups });
        }

        private static IQueryable<TQuery> UnWrapGroups<TQuery, TKey>(IQueryable<IGrouping<TKey, TQuery>> groups)
        {
            return groups.Select(g => g.First());
        }
    }
}
