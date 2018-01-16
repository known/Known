using System;
using System.ServiceModel;
using System.Web.Http;
using System.Web.Http.SelfHost;

namespace Known.WebApi
{
    class Program
    {
        static readonly Uri baseAddress = new Uri("http://localhost:9000/");

        static void Main(string[] args)
        {
            HttpSelfHostServer server = null;
            try
            {
                var config = new HttpSelfHostConfiguration(baseAddress);

                config.Routes.MapHttpRoute(
                    name: "DefaultApi",
                    routeTemplate: "api/{controller}/{id}",
                    defaults: new { id = RouteParameter.Optional }
                );

                config.MaxReceivedMessageSize = 16L * 1024 * 1024 * 1024;
                config.ReceiveTimeout = TimeSpan.FromMinutes(20);
                config.TransferMode = TransferMode.StreamedRequest;

                server = new HttpSelfHostServer(config);
                server.OpenAsync().Wait();
                Console.WriteLine("Listening on " + baseAddress);
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();

            }
            catch (Exception ex)
            {
                Console.WriteLine("Could not start server: {0}", ex.GetBaseException().Message);
                Console.WriteLine("Hit ENTER to exit...");
                Console.ReadLine();
            }
            finally
            {
                if (server != null)
                {
                    server.CloseAsync().Wait();
                }
            }
        }
    }
}
