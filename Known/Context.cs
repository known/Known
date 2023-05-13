namespace Known;

public class Context
{
    public bool IsMobile { get; set; }
    public bool IsWebApi { get; set; }
    public UserInfo CurrentUser { get; set; }
    public HttpClient Http { get; set; }

    public Task<string> GetAsync(string url)
    {
        SetTokenHeader();
        return Http.GetStringAsync(url);
    }

    public Task<TResult> GetAsync<TResult>(string url)
    {
        SetTokenHeader();
        return Http.GetFromJsonAsync<TResult>(url);
    }

    public async Task<Result> PostAsync(string url, HttpContent content = null)
    {
        SetTokenHeader();
        var response = await Http.PostAsync(url, content);
        return await response.Content.ReadFromJsonAsync<Result>();
    }

    public async Task<TResult> PostAsync<TParam, TResult>(string url, TParam data)
    {
        SetTokenHeader();
        var response = await Http.PostAsJsonAsync(url, data);
        return await response.Content.ReadFromJsonAsync<TResult>();
    }

    public Task<Result> PostAsync<T>(string url, T data) => PostAsync<T, Result>(url, data);
    public Task<PagingResult<T>> QueryAsync<T>(string url, PagingCriteria criteria) => PostAsync<PagingCriteria, PagingResult<T>>(url, criteria);

    private void SetTokenHeader()
    {
        var token = CurrentUser != null ? CurrentUser.Token : "none";
        Http.DefaultRequestHeaders.Remove(Constants.KeyToken);
        Http.DefaultRequestHeaders.Add(Constants.KeyToken, token);
    }
}