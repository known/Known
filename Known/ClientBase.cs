namespace Known;

public class ClientBase(HttpClient http) : IService
{
    private readonly HttpClient http = http;

    public Context Context { get; set; }

    public Task<string> GetAsync(string url)
    {
        SetTokenHeader();
        return http.GetStringAsync(url);
    }

    public Task<TResult> GetAsync<TResult>(string url)
    {
        SetTokenHeader();
        return http.GetFromJsonAsync<TResult>(url);
    }

    public async Task<Result> PostAsync(string url)
    {
        SetTokenHeader();
        var response = await http.PostAsync(url, null);
        return await response.Content.ReadFromJsonAsync<Result>();
    }

    public async Task<TResult> PostAsync<TParam, TResult>(string url, TParam data)
    {
        SetTokenHeader();
        var response = await http.PostAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<TResult>();
    }

    public async Task<Result> PostWithFileAsync(string url, object data)
    {
        SetTokenHeader();
        var response = await http.PostAsync(url, (HttpContent)data);
        return await response.Content.ReadFromJsonAsync<Result>();
    }

    public Task<Result> PostAsync<T>(string url, T data) => PostAsync<T, Result>(url, data);
    public Task<PagingResult<T>> QueryAsync<T>(string url, PagingCriteria criteria) => PostAsync<PagingCriteria, PagingResult<T>>(url, criteria);

    private void SetTokenHeader()
    {
        var user = Context?.CurrentUser;
        var token = user != null ? user.Token : "none";
        http.DefaultRequestHeaders.Remove(Constants.KeyToken);
        http.DefaultRequestHeaders.Add(Constants.KeyToken, token);
    }
}