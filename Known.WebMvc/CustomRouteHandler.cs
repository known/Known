using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace Known.WebMvc
{
    public class CustomRouteHandler : IRouteHandler
    {
        public IHttpHandler GetHttpHandler(RequestContext requestContext)
        {
            var routes = RouteTable.Routes;
            var controllerName = requestContext.RouteData.Values["controller"] as string;
            var actionName = requestContext.RouteData.Values["action"] as string;
            var request = requestContext.HttpContext.Request;
            var response = requestContext.HttpContext.Response;

            if (request.IsAjaxRequest() && actionName != "Partial")
            {
                switch (request.HttpMethod)
                {
                    case "GET":
                        //var result = Api.Post<ApiResult>("/api/" + route, FromJson(param));
                        break;
                    case "POST":
                        break;
                    default:
                        break;
                }
                //var serviceType = Type.GetType($"Demo.Services.{controller}Service,Demo");
                //if (serviceType != null)
                //{
                //    var service = Activator.CreateInstance(serviceType);
                //    var method = serviceType.GetMethod(action);
                //    if (method != null)
                //    {
                //        var parameters = new List<object>();
                //        foreach (var key in request.QueryString.AllKeys)
                //        {
                //            parameters.Add(request.QueryString[key]);
                //        }
                //        var result = method.Invoke(service, parameters.ToArray());
                //        response.Write(result);
                //    }

                //    response.End();
                //}
            }

            return new MvcHandler(requestContext);
        }
    }
}