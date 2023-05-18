using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Reflection;
using Ultra.Core.Extensions;
using Ultra.Core.Web.Controllers;

namespace Ultra.Core.Web.Features
{
    public class CrudControllerFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        private readonly Assembly[] _assemblies;

        public CrudControllerFeatureProvider(params Assembly[] assemblies)
        {
            _assemblies = assemblies;
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            var entities = _assemblies.GetDbEntities();

            foreach (var entity in entities)
            {
                var controller = typeof(CrudController<>).MakeGenericType(entity).GetTypeInfo();

                feature.Controllers.Add(controller);
            }
        }
    }
}
