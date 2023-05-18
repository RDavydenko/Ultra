using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using System;
using System.Linq;
using System.Linq.Expressions;

namespace Ultra.Core.DAL.Extensions
{
    public static class PropertyBuilderExtensions
    {
        public static PropertyBuilder<TState> HasEnumAsStringConversion<TState>(this PropertyBuilder<TState> builder)
            where TState : struct, Enum
            => builder.HasConversion(new ValueConverter<TState, string>(e => e.ToString(), s => Enum.Parse<TState>(s)));

        [Obsolete]
        public static PropertyBuilder HasEnumAsStringConversion(this PropertyBuilder builder, Type stateType)
        {
            // ValueConverter<stateType, string>
            var valueConverterType = typeof(ValueConverter<,>).MakeGenericType(stateType, typeof(string));

            // stateType.ToString()
            var enumToStringMethod = stateType.GetMethods().First(x => 
                x.Name == "ToString" && 
                x.GetParameters().Length == 0);
            // Enum.Parse<>()
            var enumParseMethod = typeof(Enum).GetMethods().First(x => 
                x.Name == "Parse" && 
                x.IsGenericMethod &&
                x.GetParameters().Length == 1 && 
                x.GetParameters()[0].ParameterType == typeof(string));
            // Enum.Parse<stateType>()
            var enumParseGenericMethod = enumParseMethod.MakeGenericMethod(stateType);

            var stringType = typeof(string);
            var enumParam = Expression.Parameter(stateType, "e");

            // Func<stateType, string>: e => e.ToString()
            var fromEnumToStrLambda = Expression.Lambda(
                //delegateType: typeof(Func<,>).MakeGenericType(stateType, stringType),
                Expression.Call(
                    enumParam,
                    enumToStringMethod),
                enumParam
            );

            // Func<string, stateType>: s => Enum<stateType>.Parse(s)
            var stringParam = Expression.Parameter(typeof(string), "s");
            var fromStrToEnumLambda = Expression.Lambda(
                //delegateType: typeof(Func<,>).MakeGenericType(stringType, stateType),
                Expression.Call(
                    instance: null,
                    enumParseGenericMethod,
                    stringParam),
                stringParam
            );

            // ValueConverter<stateType, string>
            var valueConverter = (ValueConverter?)Activator.CreateInstance(
                valueConverterType, 
                new object[] { fromEnumToStrLambda, fromStrToEnumLambda, null });
            return builder.HasConversion(valueConverter);
        }

        public static PropertyBuilder<TState> HasStateColumn<TState>(this PropertyBuilder<TState> builder)
            where TState : struct, Enum
            => builder.HasColumnName("STATE").HasEnumAsStringConversion();

        [Obsolete]
        public static PropertyBuilder HasStateColumn(this PropertyBuilder builder, Type stateType)
            => builder.HasColumnName("STATE").HasEnumAsStringConversion(stateType);
    }
}
