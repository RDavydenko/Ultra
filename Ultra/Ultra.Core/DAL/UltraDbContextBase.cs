using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using Ultra.Core.DAL.Extensions;
using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Tools;

namespace Ultra.Core.DAL
{
    public abstract class UltraDbContextBase : DbContext
    {
        public abstract string Schema { get; }
        private readonly Assembly _assembly;

        public UltraDbContextBase(DbContextOptions options)
            : base(options)
        {
            _assembly = Assembly.GetCallingAssembly();
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder.HasDefaultSchema(Schema);

            modelBuilder.ApplyConfigurationsFromAssembly(typeof(UltraDbContextBase).Assembly);
            modelBuilder.ApplyConfigurationsFromAssembly(Executor.WebAssembly);
            modelBuilder.ApplyConfigurationsFromAssembly(_assembly);

            var entityTypes = _assembly.GetDbEntities().ToList();

            foreach (var entityType in entityTypes)
            {
                var typeBuilder = (EntityTypeBuilder)modelBuilder.GetType().GetMethods()
                    .First(x => x.Name == "Entity" && x.IsGenericMethod && x.GetParameters().Length == 0)
                    .MakeGenericMethod(entityType)
                    .Invoke(modelBuilder, null)!;

                if (entityType.IsStatefulEntity(out var stateType))
                {
                    typeBuilder.HasStateColumn(entityType, stateType);
                }

                if (entityType.IsCrudEntity())
                {   
                    typeBuilder.HasCrudQueryFilter(entityType);
                }

                // TODO: CrudColumns,
                // TODO: Custom types
            }
        }
    }
}
