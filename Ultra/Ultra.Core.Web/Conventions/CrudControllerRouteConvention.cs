using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Ultra.Core.Web.Controllers.Abstract;

namespace Ultra.Core.Web.Conventions
{
    public class CrudControllerRouteConvention : IControllerModelConvention
    {
        public void Apply(ControllerModel controller)
        {
            if (controller.ControllerType.IsAssignableTo(typeof(ICrudController)))
            {
                var entity = controller.ControllerType.GenericTypeArguments[0];
                controller.ControllerName = $"CrudController-{entity.Name}";
                controller.Selectors.Add(new SelectorModel
                {
                    AttributeRouteModel = new AttributeRouteModel(new RouteAttribute($"crud/{entity.Name}")),
                });
            }
        }
    }
}
