using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using Ultra.Core.Services.Abstract;

namespace Ultra.Core.Services
{
    public class CrudServiceHelper : ICrudServiceHelper
    {
        private static readonly MethodInfo _countAsyncMethod;

        private readonly IServiceProvider _serviceProvider;

        static CrudServiceHelper()
        {
            _countAsyncMethod = typeof(EntityFrameworkQueryableExtensions)
                .GetMethods()
                .Where(x => x.Name == nameof(EntityFrameworkQueryableExtensions.CountAsync) && x.GetParameters().Length == 2)
                .First();
        }

        public CrudServiceHelper(IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
        }

        public async Task<int> GetCountAsync(Type entityType)
        {
            var crudService = _serviceProvider.GetRequiredService(GetCrudServiceType(entityType));

            var task = (Task)crudService.GetType().GetMethod("GetQueryable").Invoke(crudService, null);
            await task.ConfigureAwait(false);
            var resultProperty = task.GetType().GetProperty("Result");
            var queryable = resultProperty.GetValue(task);

            return await (Task<int>)_countAsyncMethod
                .MakeGenericMethod(entityType)
                .Invoke(null, new object[] { queryable, CancellationToken.None });
        }

        private Type GetCrudServiceType(Type entityType)
            => typeof(ICrudService<>).MakeGenericType(entityType);
    }
}
