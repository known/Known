/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

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