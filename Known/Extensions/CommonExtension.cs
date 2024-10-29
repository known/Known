namespace Known.Extensions;

/// <summary>
/// 通用扩展类。
/// </summary>
public static class CommonExtension
{
    #region String
    internal static void AppendLine(this StringBuilder sb, string format, params object[] args)
    {
        var value = string.Format(format, args);
        sb.AppendLine(value);
    }

    internal static string Format(this string format, params object[] args) => string.Format(format, args);
    #endregion

    #region Enum
    /// <summary>
    /// 获取枚举字段描述。
    /// </summary>
    /// <param name="value">枚举字段值。</param>
    /// <returns>枚举字段描述。</returns>
    public static string GetDescription(this Enum value)
    {
        var type = value.GetType();
        var name = Enum.GetName(type, value);
        if (string.IsNullOrWhiteSpace(name))
            return string.Empty;

        var field = type.GetField(name);
        var attr = field.GetCustomAttribute<DescriptionAttribute>();
        return attr != null ? attr.Description : name;
    }
    #endregion

    #region Object
    /// <summary>
    /// 合并两个对象。
    /// </summary>
    /// <param name="obj1">对象1。</param>
    /// <param name="obj2">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static object Merge(this object obj1, object obj2)
    {
        if (obj1 == null) return null;
        if (obj2 == null) return obj1;

        var obj1Type = obj1.GetType();
        var obj2Type = obj2.GetType();
        var obj1Properties = obj1Type.GetProperties();
        var obj2Properties = obj2Type.GetProperties();

        var keyValues = new Dictionary<string, Type>();
        foreach (var prop in obj1Properties)
            keyValues[prop.Name] = prop.PropertyType;
        foreach (var prop in obj2Properties)
            keyValues[prop.Name] = prop.PropertyType;

        var mergedType = TypeHelper.CreateType(keyValues);
        var mergedObject = Activator.CreateInstance(mergedType);

        foreach (var property in obj1Properties)
        {
            var value = obj1Type.GetProperty(property.Name).GetValue(obj1, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        foreach (var property in obj2Properties)
        {
            var value = obj2Type.GetProperty(property.Name).GetValue(obj2, null);
            mergedType.GetProperty(property.Name).SetValue(mergedObject, value, null);
        }

        return mergedObject;
    }

    /// <summary>
    /// 合并两个泛型对象，返回动态对象。
    /// </summary>
    /// <typeparam name="TLeft">对象1类型。</typeparam>
    /// <typeparam name="TRight">对象2类型。</typeparam>
    /// <param name="left">对象1。</param>
    /// <param name="right">对象2。</param>
    /// <returns>合并后的对象。</returns>
    public static ExpandoObject Merge<TLeft, TRight>(this TLeft left, TRight right)
    {
        var expando = new ExpandoObject();
        IDictionary<string, object> dict = expando;
        foreach (var p in typeof(TLeft).GetProperties())
            dict[p.Name] = p.GetValue(left);
        foreach (var p in typeof(TRight).GetProperties())
            dict[p.Name] = p.GetValue(right);
        return expando;
    }
    #endregion

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
        navigation.NavigateTo($"/error/{code}");
    }

    /// <summary>
    /// 导航到登录页面。
    /// </summary>
    /// <param name="navigation">导航管理者对象。</param>
    public static void GoLoginPage(this NavigationManager navigation)
    {
        navigation.NavigateTo("/login", true);
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
    #endregion

    #region File
    internal static async Task<FileDataInfo> ReadFileAsync(this IBrowserFile file)
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
    #endregion
}