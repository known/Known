using Microsoft.AspNetCore.Mvc.ActionConstraints;
using Microsoft.AspNetCore.Mvc.ApplicationModels;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Known;

class ApiConvention : IApplicationModelConvention
{
    public void Apply(ApplicationModel application)
    {
        foreach (var item in application.Controllers)
        {
            ConfigureApiExplorer(item);
            ConfigureSelector(item);
        }
    }

    private static void ConfigureApiExplorer(ControllerModel model)
    {
        if (!model.ApiExplorer.IsVisible.HasValue)
            model.ApiExplorer.IsVisible = true;

        foreach (var action in model.Actions)
        {
            if (!action.ApiExplorer.IsVisible.HasValue)
            {
                action.ApiExplorer.IsVisible = true;
            }
        }
    }

    private static void ConfigureSelector(ControllerModel model)
    {
        RemoveEmptySelectors(model.Selectors);

        if (model.Selectors.Any(selector => selector.AttributeRouteModel != null))
            return;

        foreach (var action in model.Actions)
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

    private static void ConfigureSelector(ActionModel model)
    {
        RemoveEmptySelectors(model.Selectors);

        var route = GetRouteTemplate(model);
        var method = GetHttpMethod(model);
        if (model.Selectors.Count <= 0)
            AddServiceSelector(route, method, model);
        else
            NormalizeSelectorRoutes(route, method, model);

        if (method == "POST")
        {
            foreach (var item in model.Parameters)
            {
                if (item.ParameterType.IsClass && item.ParameterType != typeof(string))
                {
                    Console.WriteLine(item.ParameterType);
                    item.BindingInfo = BindingInfo.GetBindingInfo([new FromBodyAttribute()]);
                }
            }
        }
    }

    private static void AddServiceSelector(string route, string method, ActionModel model)
    {
        var template = new RouteAttribute(route);
        var selector = new SelectorModel
        {
            AttributeRouteModel = new AttributeRouteModel(template)
        };
        selector.ActionConstraints.Add(new HttpMethodActionConstraint([method]));
        model.Selectors.Add(selector);
    }

    private static void NormalizeSelectorRoutes(string route, string method, ActionModel model)
    {
        foreach (var selector in model.Selectors)
        {
            if (selector.AttributeRouteModel == null)
            {
                var template = new RouteAttribute(route);
                selector.AttributeRouteModel = new AttributeRouteModel(template);
            }

            if (selector.ActionConstraints.OfType<HttpMethodActionConstraint>().FirstOrDefault()?.HttpMethods?.FirstOrDefault() == null)
                selector.ActionConstraints.Add(new HttpMethodActionConstraint([method]));
        }
    }

    private static string GetRouteTemplate(ActionModel model)
    {
        if (model.Attributes != null && model.Attributes.Count > 0)
        {
            foreach (var item in model.Attributes)
            {
                if (item is RouteAttribute attribute)
                {
                    return attribute.Template;
                }
            }
        }

        var route = new StringBuilder();
        route.Append("api");
        // 二级路径
        //var names = model.Controller.ControllerType.Namespace.Split('.');
        //if (names.Length > 2)
        //    route.Append(names[^2]);

        // Controller
        var controllerName = model.Controller.ControllerName;
        if (controllerName.EndsWith("Service"))
            controllerName = controllerName[0..^7];

        route.Append($"/{controllerName}");

        // Action
        var actionName = model.ActionName;
        if (actionName.EndsWith("Async"))
            actionName = actionName[..^"Async".Length];

        if (!string.IsNullOrEmpty(actionName))
            route.Append($"/{actionName}");

        //Console.WriteLine(route);
        return route.ToString();
    }

    private static string GetHttpMethod(ActionModel model)
    {
        var actionName = model.ActionName;
        if (actionName.StartsWith("Get"))
            return "GET";

        return "POST";
    }
}