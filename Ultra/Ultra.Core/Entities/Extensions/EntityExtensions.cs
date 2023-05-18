using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Ultra.Core.Entities.Attributes.Access;
using Ultra.Core.Entities.Attributes.Access.Abstract;
using Ultra.Core.Entities.Attributes.Security;
using Ultra.Core.Entities.Interfaces;
using Ultra.Core.Extensions;
using Ultra.Core.Models;
using Ultra.Core.Extensions;
using Ultra.Extensions;
using Microsoft.CodeAnalysis.CSharp.Syntax;
using Ultra.Core.Entities.Attributes;

namespace Ultra.Core.Entities.Extensions
{
    public static class EntityExtensions
    {
        #region IsAvailableTo...

        public static bool IsAvailableToRead(this IEntity entity) => entity.GetType().IsAvailableToRead();
        public static bool IsAvailableToCreate(this IEntity entity) => entity.GetType().IsAvailableToCreate();
        public static bool IsAvailableToUpdate(this IEntity entity) => entity.GetType().IsAvailableToUpdate();
        //public static bool IsAvailableToPatch(this IEntity entity) => entity.GetType().IsAvailableToPatch();
        public static bool IsAvailableToDelete(this IEntity entity) => entity.GetType().IsAvailableToDelete();

        public static bool IsAvailableToRead(this Type type) => type.IsAvailableTo<ReadAttribute>();
        public static bool IsAvailableToCreate(this Type type) => type.IsAvailableTo<CreateAttribute>();
        public static bool IsAvailableToUpdate(this Type type) => type.IsAvailableTo<UpdateAttribute>();
        //public static bool IsAvailableToPatch(this Type type) => type.IsAvailableTo<PatchAttribute>();
        public static bool IsAvailableToDelete(this Type type) => type.IsAvailableTo<DeleteAttribute>();

        private static bool IsAvailableTo<TAttribute>(this Type type)
            where TAttribute : AccessAttributeBase
        {
            return type.GetAttribute<TAttribute>()?.IsAvailable != true;
        }

        #endregion

        #region ToInputModels

        public static T ToCreated<T>(this T model)
            where T : class, IEntity
        {
            typeof(T).GetProperties()
                .Where(p => p.CanWrite && !p.IsAvailableToCreated())
                .ForEach(p => p.SetValue(model, null, null));

            return model;
        }

        public static T ToUpdated<T>(this T model)
            where T : class, IEntity
        {
            typeof(T).GetProperties()
                .Where(p => p.CanWrite && !p.IsAvailableToUpdated())
                .ForEach(p => p.SetValue(model, null, null));

            return model;
        }

        public static PatchModel<T> ToPatched<T>(this PatchModel<T> model)
            where T : class, IEntity
        {
            var availableKeys = typeof(T).GetProperties()
                .Where(p => p.CanWrite && p.IsAvailableToPatched())
                .Select(p => p.Name)
                .ToList();

            foreach(var key in model.Values.Keys.ToList())
            {
                if (!availableKeys.Contains(key))
                {
                    model.Values.Remove(key);
                }
            }

            return model;
        }

        #endregion

        #region HiddenFields

        public static TEntity RemoveHidden<TEntity>(this TEntity entity)
            where TEntity : class, IEntity
        {
            if (entity == null)
                return entity;

            typeof(TEntity).GetProperties()
                .Where(p => p.CanWrite && p.IsHidden())
                .ForEach(p => p.SetValue(entity, null, null));

            return entity;
        }

        public static IEnumerable<TEntity> RemoveHidden<TEntity>(this IEnumerable<TEntity> entities)
            where TEntity : class, IEntity
        {
            if (entities == null)
                return entities;

            entities.ForEach(entity => entity.RemoveHidden());

            return entities;
        }

        #endregion

        #region Implementation

        public static bool IsDbEntity(this Type type)
        {
            return type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IDbEntity));
        }

        public static bool IsEntity(this Type type)
        {
            return type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IEntity));
        }

        public static bool IsCrudEntity(this Type type)
        {
            return type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(ICrudEntity));
        }

        public static bool IsStatefulEntity(this Type type, out Type stateType)
        {
            stateType = null;

            if (type.IsClass && !type.IsAbstract)
            {
                var intss = type.GetInterfaces();
                var statefulType = type.GetInterfaces().FirstOrDefault(x => 
                    x.IsGenericType &&
                    x.GetGenericTypeDefinition() == typeof(IStatefulEntity<>));
                if(statefulType == null)
                {
                    return false;
                }
                stateType = statefulType.GetGenericArguments()[0];
                return true;
            }

            return false;
        }

        public static bool IsDisplayableEntity(this Type type, out string displayFieldName)
        {
            //return type.IsClass && !type.IsAbstract && type.IsAssignableTo(typeof(IDisplayableEntity));
            displayFieldName = null;

            var displayAttr = type.GetCustomAttribute<DisplayAttribute>();
            if (displayAttr != null && displayAttr.DisplayableField != null)
            {
                displayFieldName = displayAttr.DisplayableField;
                return true;
            }

            return false;
        }

        #endregion
    }
}
