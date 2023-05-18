using HotChocolate.Resolvers;
using System.Linq;
using System.Threading.Tasks;

namespace Ultra.Core.Web.GraphQL.Distinct
{
    public class UseDistinctMiddleware
    {
        private FieldDelegate _next;

        public UseDistinctMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            await _next.Invoke(context);

            var distinct = context.ArgumentValue<bool?>(UseDistinctConstants.ArgumentName);
            if (distinct != true)
            {
                return;
            }

            if (context.Result is not IQueryable query)
            {
                return;
            }

            context.Result = ApplyDistinct(query);
        }

        private static IQueryable ApplyDistinct(IQueryable query)
        {
            /*
                query.Distinct()
             */

            var entityType = query.ElementType;
            var distinctMethod = typeof(Queryable).GetMethods()
                .First(m =>
                    m.Name == "Distinct" &&
                    m.IsGenericMethod &&
                    m.GetParameters().Length == 1);
            var distinctGenericMethod = distinctMethod.MakeGenericMethod(entityType);

            return (IQueryable)distinctGenericMethod.Invoke(null, new object[] { query });
        }
    }
}
