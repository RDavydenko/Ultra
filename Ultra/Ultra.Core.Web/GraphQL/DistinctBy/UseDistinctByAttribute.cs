using HotChocolate.Resolvers;
using HotChocolate.Types.Descriptors;
using HotChocolate.Types;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using HotChocolate.Types.Descriptors.Definitions;

namespace Ultra.Core.Web.GraphQL.DistinctBy
{
    public class UseDistinctByAttribute : ObjectFieldDescriptorAttribute
    {
        public UseDistinctByAttribute([CallerLineNumber] int order = 0)
        {
            Order = order;
        }

        public override void OnConfigure(IDescriptorContext context,
            IObjectFieldDescriptor descriptor, MemberInfo member)
        {
            var typeRef = context.TypeInspector.GetReturnType(member);
            var returnType = typeRef.Type;
            var entityType = returnType.GetGenericArguments()[0];
            var inputType = typeof(ObjectPropertyType<>).MakeGenericType(entityType);
            var middleware = FieldClassMiddlewareFactory.Create(typeof(UseDistinctByMiddleware<>).MakeGenericType(entityType));
            FieldMiddleware placeholder = next => context => new ValueTask(Task.CompletedTask);
            descriptor.Argument(UseDistinctByConstants.ArgumentName, a => a.Type(inputType))
                .Use(middleware)
                .Extend()
                .OnBeforeCompletion((context, definition) =>
                {
                    definition.MiddlewareDefinitions.Add(new FieldMiddlewareDefinition(middleware, true, "UseDistinctBy"));
                });
        }
    }
}
