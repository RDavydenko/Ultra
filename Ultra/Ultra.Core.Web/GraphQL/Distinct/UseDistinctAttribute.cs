using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HotChocolate.Types.Descriptors.Definitions;
using System.Security.Cryptography;

namespace Ultra.Core.Web.GraphQL.Distinct
{
    public class UseDistinctAttribute : ObjectFieldDescriptorAttribute
    {
        public UseDistinctAttribute([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        //public override void OnConfigure(IDescriptorContext context,
        //    IObjectFieldDescriptor descriptor, MemberInfo member)
        //{
        //    var typeRef = context.TypeInspector.GetReturnType(member);
        //    var returnType = typeRef.Type;
        //    var entityType = returnType.GetGenericArguments()[0];
        //    var inputType = typeof(BooleanType);
        //    var middleware = FieldClassMiddlewareFactory.Create(typeof(UseDistinctMiddleware<>).MakeGenericType(entityType));
        //    FieldMiddleware placeholder = next => context => new ValueTask(Task.CompletedTask);
        //    descriptor.Argument(UseDistinctConstants.ArgumentName, a => a.Type(inputType))
        //        .Use(middleware)
        //        .Extend()
        //        .OnBeforeCompletion((context, definition) =>
        //        {
        //            definition.MiddlewareDefinitions.Add(new FieldMiddlewareDefinition(middleware, false, "UseDistinct"));
        //        });
        //}

        public override void OnConfigure(IDescriptorContext context, IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            descriptor
                    .Argument(UseDistinctConstants.ArgumentName, a => a.Type<BooleanType>())
                    .Use<UseDistinctMiddleware>();
        }
    }
}
