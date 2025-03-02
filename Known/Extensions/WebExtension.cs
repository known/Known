namespace Known.Extensions;

/// <summary>
/// Web对象扩展类。
/// </summary>
public static class WebExtension
{
    #region Navigation
    /// <summary>
    /// 获取页面路由BaseUri后面的相对URL。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <returns>相对URL。</returns>
    public static string GetPageUrl(this NavigationManager navigation)
    {
        var baseUrl = navigation.BaseUri.TrimEnd('/');
        return navigation.Uri.Replace(baseUrl, "");
    }

    /// <summary>
    /// 导航到指定错误码的错误页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="code">错误页面代码。</param>
    public static void GoErrorPage(this NavigationManager navigation, string code)
    {
        navigation?.NavigateTo($"/error/{code}");
    }

    /// <summary>
    /// 导航到安装页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public static void GoInstallPage(this NavigationManager navigation, bool forceLoad = false)
    {
        navigation?.NavigateTo("/install", forceLoad);
    }

    /// <summary>
    /// 导航到登录页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public static void GoLoginPage(this NavigationManager navigation, bool forceLoad = false)
    {
        navigation?.NavigateTo("/login", forceLoad);
    }

    /// <summary>
    /// 导航到首页。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="forceLoad">是否强制刷新。</param>
    public static void GoHomePage(this NavigationManager navigation, bool forceLoad = false)
    {
        navigation?.NavigateTo("/", forceLoad);
    }
    #endregion

    #region File
    /// <summary>
    /// 异步创建浏览器上传的附件，如果是图片，则压缩高清图后再上传。
    /// </summary>
    /// <param name="file">浏览器附件对象。</param>
    /// <param name="isCompress">是否压缩图片。</param>
    /// <param name="size">压缩图片大小。</param>
    /// <returns>附件数据对象。</returns>
    public static async Task<FileDataInfo> CreateFileAsync(this IBrowserFile file, bool isCompress = true, Size? size = null)
    {
        if (!Utils.CheckImage(file.Name) || !isCompress)
            return await file.ReadFileAsync();

        if (size == null)
            size = new Size(1920, 1080);
        var width = size.Value.Width;
        var height = size.Value.Height;
        var format = file.ContentType;
        var image = await file.RequestImageFileAsync(format, width, height);
        return await image.ReadFileAsync();
    }

    /// <summary>
    /// 异步读取浏览器上传的附件。
    /// </summary>
    /// <param name="file">浏览器附件对象。</param>
    /// <returns>附件数据对象。</returns>
    public static async Task<FileDataInfo> ReadFileAsync(this IBrowserFile file)
    {
        using var stream = new MemoryStream();
        await file.OpenReadStream(Config.App.UploadMaxSize).CopyToAsync(stream);
        var bytes = stream.GetBuffer();
        return new FileDataInfo
        {
            Name = file.Name,
            Size = file.Size,
            Bytes = bytes
        };
    }
    #endregion
}