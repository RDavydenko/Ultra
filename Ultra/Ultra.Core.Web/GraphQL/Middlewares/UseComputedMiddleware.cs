using DelegateDecompiler;
using HotChocolate.Resolvers;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Ultra.Core.Web.GraphQL.Middlewares
{
    public class UseComputedMiddleware
    {
        private FieldDelegate _next;

        public UseComputedMiddleware(FieldDelegate next)
        {
            _next = next;
        }

        public async Task InvokeAsync(IMiddlewareContext context)
        {
            await _next.Invoke(context);

            System.Console.WriteLine("HELLLLLLLLLLLLLLLLLLLLLOOOOOOOOOOOOOOO");

            if (!(context.Result is IQueryable query))
            {
                return;
            }

            System.Console.WriteLine("222222222222222222HELLLLLLLLLLLLLLLLLLLLLOOOOOOOOOOOOOOO");

            context.Result = query.Decompile();
        }
    }
}
