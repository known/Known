namespace Known.Helpers;

public sealed class HttpHelper
{
    private HttpHelper() { }

    public static string Get(string url, string token = null, string proxyUrl = null, Encoding encoding = null)
    {
        using var client = GetWebClient(token, proxyUrl);
        if (encoding == null)
            return client.GetStringAsync(url).Result;

        var bytes = client.GetByteArrayAsync(url).Result;
        return encoding.GetString(bytes);
    }

    public static string Post(string url, object data = null, string token = null, string proxyUrl = null, Encoding encoding = null)
    {
        if (data == null)
        {
            using var client = GetWebClient(token, proxyUrl);
            var result = client.PostAsync(url, null).Result.Content;
            if (encoding == null)
                return result.ReadAsStringAsync().Result;

            var bytes = result.ReadAsByteArrayAsync().Result;
            return encoding.GetString(bytes);
        }
        else
        {
            string contentType;
            string postData;
            if (data.GetType() == typeof(string))
            {
                contentType = "application/x-www-form-urlencoded";
                postData = data.ToString();
            }
            else
            {
                contentType = "application/json";
                postData = Utils.ToJson(data);
            }

            using var client = GetWebClient(token, proxyUrl);
            using var content = new StringContent(postData, Encoding.UTF8, contentType);
            var result = client.PostAsync(url, content).Result.Content;
            if (encoding == null)
                return result.ReadAsStringAsync().Result;

            var bytes = result.ReadAsByteArrayAsync().Result;
            return encoding.GetString(bytes);
        }
    }

    private static HttpClient GetWebClient(string token = null, string proxyUrl = null)
    {
        var client = new HttpClient();
        if (!string.IsNullOrEmpty(token))
            client.DefaultRequestHeaders.Add(Constants.KeyToken, token);

        return client;
    }
}