using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Known;

class ApiFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeof(IService).IsAssignableFrom(typeInfo) ||
            !typeInfo.IsPublic ||
            typeInfo.IsAbstract ||
            typeInfo.IsGenericType)
            return false;

        return true;
    }
}
