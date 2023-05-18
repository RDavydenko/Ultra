using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Ultra.Core.Entities.Extensions;

namespace Ultra.Core.Extensions
{
    public static class AssemblyExtensions
    {
        public static IEnumerable<Type> GetDbEntities(this Assembly? assembly)
        {
            if (assembly == null)
                return Enumerable.Empty<Type>();

            return assembly.GetExportedTypes()
                .Where(x => x.IsDbEntity())
                .ToList();
        }

        public static IEnumerable<Type> GetDbEntities(this Assembly[] assemblies)
        {
            return assemblies.SelectMany(x => GetDbEntities(x)).ToList();
        }
    }
}
