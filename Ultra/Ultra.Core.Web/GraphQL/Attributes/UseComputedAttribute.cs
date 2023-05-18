using DelegateDecompiler;
using HotChocolate.Types;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types.Pagination;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Web.GraphQL.Middlewares;

namespace Ultra.Core.Web.GraphQL.Attributes
{
    public class UseComputedAttribute : ObjectFieldDescriptorAttribute
    {
        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            //if (descriptor is IObjectFieldDescriptor ofd)
            //{
            //    ofd.Use<UseComputedMiddleware>();
            //}

            descriptor.Use(next => async context =>
            {
                await next(context).ConfigureAwait(false);
                if (context.Result is IQueryable queryable)
                    context.Result = queryable.Decompile();

            });
        }
    }
}
