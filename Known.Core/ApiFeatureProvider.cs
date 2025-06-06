﻿using Microsoft.AspNetCore.Mvc.Controllers;

namespace Known;

class ApiFeatureProvider : ControllerFeatureProvider
{
    protected override bool IsController(TypeInfo typeInfo)
    {
        if (typeInfo.GetCustomAttribute<WebApiAttribute>() == null)
            return false;

        return true;
    }
}