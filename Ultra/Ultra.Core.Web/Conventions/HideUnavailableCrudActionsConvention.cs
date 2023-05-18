using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using System;
using System.Collections.Generic;
using System.Reflection;
using Ultra.Core.Entities.Abstract;
using Ultra.Core.Entities.Attributes.Access;
using Ultra.Core.Entities.Extensions;
using Ultra.Core.Web.Controllers;
using Ultra.Core.Web.Controllers.Abstract;
using Ultra.Core.Extensions;

namespace Ultra.Core.Web.Conventions
{
    public class HideUnavailableCrudActionsConvention : IApplicationModelConvention
    {
        public void Apply(ApplicationModel application)
        {
            foreach (var controller in application.Controllers)
            {
                if (!controller.ControllerType.IsAssignableTo(typeof(ICrudController)))
                {
                    continue;
                }

                var entity = controller.ControllerType.GenericTypeArguments[0];
                var toBeRemoved = new List<ActionModel>();
                foreach (var action in controller.Actions)
                {
                    if (ShouldBeRemoved(entity, action))
                    {
                        toBeRemoved.Add(action);
                    }
                }

                foreach (var action in toBeRemoved)
                {
                    controller.Actions.Remove(action);
                }
            }
        }

        private bool ShouldBeRemoved(Type entity, ActionModel action)
        {
            if (action.ActionName == nameof(CrudController<EntityBase>.GetAll))
            {
                return entity.IsAvailableToRead();
            }

            if (action.ActionName == nameof(CrudController<EntityBase>.GetById))
            {
                return entity.IsAvailableToRead();
            }

            if (action.ActionName == nameof(CrudController<EntityBase>.Create))
            {
                return entity.IsAvailableToCreate();
            }

            if (action.ActionName == nameof(CrudController<EntityBase>.Update))
            {
                return entity.IsAvailableToUpdate();
            }

            if (action.ActionName == nameof(CrudController<EntityBase>.Patch))
            {
                return entity.IsAvailableToUpdate();
            }

            if (action.ActionName == nameof(CrudController<EntityBase>.Delete))
            {
                return entity.IsAvailableToDelete();
            }

            return false;
        }
    }
}
