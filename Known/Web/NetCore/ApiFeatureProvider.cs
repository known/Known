#if NET6_0
using System.Reflection;
using Microsoft.AspNetCore.Mvc.Controllers;

namespace Known.Web;

class ApiFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (!typeof(IService).IsAssignableFrom(typeInfo) ||
            typeInfo.IsAbstract ||
            typeInfo.IsGenericType)
            return false;

        return true;
    }
}
#endif