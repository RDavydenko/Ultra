using HotChocolate.Types;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Linq;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Entities.States;

namespace Ultra.Core.DAL.Extensions
{
    public static class EntityConfigurationExtensions
    {
        public static void HasIdColumn<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IEntity
        {
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).HasColumnName("ID").IsRequired();
        }

        public static void HasGuidColumn<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IGuidEntity
        {
            builder.HasKey(x => x.Guid);
            builder.Property(x => x.Guid).HasColumnName("GUID").IsRequired();
        }

        public static void HasCrudColumns<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ICrudEntity
        {
            builder.HasIdColumn();
            builder.Property(x => x.CreateUserId).HasColumnName("CREATE_USER_ID").IsRequired();
            builder.Property(x => x.CreateDate).HasColumnName("CREATE_DATE").IsRequired();
            builder.Property(x => x.UpdateUserId).HasColumnName("UPDATE_USER_ID");
            builder.Property(x => x.UpdateDate).HasColumnName("UPDATE_DATE");
            builder.Property(x => x.DeleteUserId).HasColumnName("DELETE_USER_ID");
            builder.Property(x => x.DeleteDate).HasColumnName("DELETE_DATE");
        }

        public static void HasStateColumn<TEntity, TState>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, IStatefulEntity<TState>
            where TState : struct, Enum
        {
            builder.Property(x => x.State).HasStateColumn();
        }

        public static void HasStateColumn(this EntityTypeBuilder builder, Type entityType, Type stateType)
        {
            typeof(EntityConfigurationExtensions).GetMethods()
                .First(x => x.Name == nameof(HasStateColumn) && x.IsGenericMethod && x.GetParameters().Length == 1)
                .MakeGenericMethod(entityType, stateType)
                .Invoke(null, new object[] { builder });
        }

        public static void HasCrudQueryFilter<TEntity>(this EntityTypeBuilder<TEntity> builder)
            where TEntity : class, ICrudEntity
        {
            builder.HasQueryFilter(x => x.DeleteUserId == null);
        }

        public static void HasCrudQueryFilter(this EntityTypeBuilder builder, Type entityType)
        {
            typeof(EntityConfigurationExtensions).GetMethods()
                .First(x => x.Name == nameof(HasCrudQueryFilter) && x.IsGenericMethod && x.GetParameters().Length == 1)
                .MakeGenericMethod(entityType)
                .Invoke(null, new object[] { builder });
        }
    }
}
