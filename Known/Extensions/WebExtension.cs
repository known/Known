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
    public static void GoInstallPage(this NavigationManager navigation)
    {
        navigation?.NavigateTo("/install", true);
    }

    /// <summary>
    /// 导航到登录页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    public static void GoLoginPage(this NavigationManager navigation)
    {
        navigation?.NavigateTo("/login", true);
    }

    /// <summary>
    /// 导航到指定菜单对应的页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="item">跳转的菜单对象。</param>
    public static void NavigateTo(this NavigationManager navigation, MenuInfo item)
    {
        if (item == null)
            return;

        //缓存APP代码中添加的菜单
        UIConfig.SetMenu(item);
        navigation.NavigateTo(item.RouteUrl);
    }

    /// <summary>
    /// 导航到指定动作对应的页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    /// <param name="item">跳转的动作对象。</param>
    public static void NavigateTo(this NavigationManager navigation, ActionInfo item)
    {
        if (item == null)
            return;

        var menu = new MenuInfo { Id = item.Id, Name = item.Name, Icon = item.Icon, Url = item.Url };
        navigation.NavigateTo(menu);
    }
    #endregion

    #region File
    /// <summary>
    /// 异步创建浏览器上传的附件，如果是图片，则压缩高清图后再上传。
    /// </summary>
    /// <param name="file">浏览器附件对象。</param>
    /// <returns>附件数据对象。</returns>
    public static async Task<FileDataInfo> CreateFileAsync(this IBrowserFile file)
    {
        if (!Utils.CheckImage(file.Name))
            return await file.ReadFileAsync();

        var format = file.ContentType;
        var image = await file.RequestImageFileAsync(format, 1920, 1080);
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