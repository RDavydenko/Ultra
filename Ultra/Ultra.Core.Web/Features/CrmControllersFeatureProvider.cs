using Microsoft.AspNetCore.Mvc.ApplicationParts;
using Microsoft.AspNetCore.Mvc.Controllers;
using System.Collections.Generic;
using System.Reflection;
using Ultra.Core.Web.Controllers;

namespace Ultra.Core.Web.Features
{
    public class CrmControllersFeatureProvider : IApplicationFeatureProvider<ControllerFeature>
    {
        public CrmControllersFeatureProvider()
        {
        }

        public void PopulateFeature(IEnumerable<ApplicationPart> parts, ControllerFeature feature)
        {
            feature.Controllers.Add(typeof(EntitiesController).GetTypeInfo());
        }
    }
}
