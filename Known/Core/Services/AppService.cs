namespace Known.Core.Services
{
    public class AppService : ServiceBase
    {
        public AppService(Context context) : base(context)
        {
        }

        public string GetApiUrl(string apiId)
        {
            var apiUrl = string.Empty;
            if (apiId == "plt")
                apiUrl = "http://localhost:8089";

            return apiUrl;
        }
    }
}
