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
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;

namespace Known.Web;

class ApiConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var controller in application.Controllers)
        {
            var type = controller.ControllerType;
            if (typeof(IService).IsAssignableFrom(type))
            {
                ConfigureApiExplorer(controller);
                ConfigureSelector(controller);
                //ConfigureParameters(controller);
            }
        }
    }

    private static void ConfigureApiExplorer(ControllerModel controller)
    {
        if (!controller.ApiExplorer.IsVisible.HasValue)
            controller.ApiExplorer.IsVisible = true;

        foreach (var action in controller.Actions)
        {
            if (!action.ApiExplorer.IsVisible.HasValue)
            {
                action.ApiExplorer.IsVisible = true;
            }
        }
    }

#region ConfigureSelector
    private void ConfigureSelector(ControllerModel controller)
    {
        RemoveEmptySelectors(controller.Selectors);

        if (controller.Selectors.Any(selector => selector.AttributeRouteModel != null))
            return;

        foreach (var action in controller.Actions)
        {
            ConfigureSelector(action);
        }
    }

    private static void RemoveEmptySelectors(IList<SelectorModel> selectors)
    {
        for (var i = selectors.Count - 1; i >= 0; i--)
        {
            var selector = selectors[i];
            if (selector.AttributeRouteModel == null &&
               (selector.ActionConstraints == null || selector.ActionConstraints.Count <= 0) &&
               (selector.EndpointMetadata == null || selector.EndpointMetadata.Count <= 0))
            {
                selectors.Remove(selector);
            }
        }
    }

    private void ConfigureSelector(ActionModel action)
    {
        RemoveEmptySelectors(action.Selectors);

        if (action.Selectors.Count <= 0)
            AddServiceSelector(action);
        else
            NormalizeSelectorRoutes(action);
    }

    private void AddServiceSelector(ActionModel action)
    {
        var selector = new SelectorModel();
        selector.AttributeRouteModel = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(GetRouteTemplate(action)));
        selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
        action.Selectors.Add(selector);
    }

    private void NormalizeSelectorRoutes(ActionModel action)
    {
        foreach (var selector in action.Selectors)
        {
            if (selector.AttributeRouteModel == null)
                selector.AttributeRouteModel = new AttributeRouteModel(new Microsoft.AspNetCore.Mvc.RouteAttribute(GetRouteTemplate(action)));

            if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
                selector.ActionConstraints.Add(new HttpMethodActionConstraint(new[] { GetHttpMethod(action) }));
        }
    }

    private string GetRouteTemplate(ActionModel action)
    {
        if (action.Attributes != null && action.Attributes.Count > 0)
        {
            foreach (var item in action.Attributes)
            {
                if (item is Known.RouteAttribute)
                {
                    return (item as Known.RouteAttribute).Path;
                }
            }
        }

        var routeTemplate = new StringBuilder();
        //routeTemplate.Append("api");
        var names = action.Controller.ControllerType.Namespace.Split('.');
        if (names.Length > 2)
        {
            routeTemplate.Append(names[^2]);
        }

        // Controller
        var controllerName = action.Controller.ControllerName;
        if (controllerName.EndsWith("Service"))
            controllerName = controllerName[0..^7];

        routeTemplate.Append($"/{controllerName}");

        // Action 名称部分
        var actionName = action.ActionName;
        if (actionName.EndsWith("Async"))
            actionName = actionName.Substring(0, actionName.Length - "Async".Length);

        //var trimPrefixes = new[]
        //{
        //    "GetAll", "GetList", "Get",
        //    "Post", "Create", "Add", "Insert",
        //    "Put", "Update",
        //    "Delete", "Remove",
        //    "Patch"
        //};

        //foreach (var trimPrefix in trimPrefixes)
        //{
        //    if (actionName.StartsWith(trimPrefix))
        //    {
        //        actionName = actionName[trimPrefix.Length..];
        //        break;
        //    }
        //}

        if (!string.IsNullOrEmpty(actionName))
            routeTemplate.Append($"/{actionName}");

        // id 部分
        //if (action.Parameters.Any(temp => temp.ParameterName == "id"))
        //    routeTemplate.Append("/{id}");

        return routeTemplate.ToString();
    }

    private static string GetHttpMethod(ActionModel action)
    {
        var actionName = action.ActionName;
        if (actionName.StartsWith("Get"))
            return "GET";

        //if (actionName.StartsWith("Put") || actionName.StartsWith("Update"))
        //    return "PUT";

        //if (actionName.StartsWith("Delete") || actionName.StartsWith("Remove"))
        //    return "DELETE";

        //if (actionName.StartsWith("Patch"))
        //    return "PATCH";

        return "POST";
    }
#endregion

    private static void ConfigureParameters(ControllerModel controller)
    {
        foreach (var action in controller.Actions)
        {
            foreach (var parameter in action.Parameters)
            {
                if (parameter.BindingInfo != null)
                    continue;

                if (parameter.ParameterType.IsClass &&
                    parameter.ParameterType != typeof(string) &&
                    parameter.ParameterType != typeof(IFormFile))
                {
                    var httpMethods = action.Selectors
                        .SelectMany(temp => temp.ActionConstraints)
                        .OfType<HttpMethodActionConstraint>()
                        .SelectMany(temp => temp.HttpMethods)
                        .ToList();
                    if (httpMethods.Contains("GET") ||
                        httpMethods.Contains("DELETE") ||
                        httpMethods.Contains("TRACE") ||
                        httpMethods.Contains("HEAD"))
                        continue;

                    //parameter.BindingInfo = BindingInfo.GetBindingInfo(new[] { new FromBodyAttribute() });
                }
            }
        }
    }
}
#endif