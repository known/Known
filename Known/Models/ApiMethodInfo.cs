namespace Known.Models;

/// <summary>
/// WebApi方法信息类。
/// </summary>
public class ApiMethodInfo
{
    /// <summary>
    /// 取得或设置方法ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置方法路由地址。
    /// </summary>
    [DisplayName("路由")]
    public string Route { get; set; }

    /// <summary>
    /// 取得或设置方法描述。
    /// </summary>
    [DisplayName("描述")]
    public string Description { get; set; }

    /// <summary>
    /// 取得或设置方法HTTP请求方式，默认方法名以Get开头的方法为GET请求，其他为POST请求。
    /// </summary>
    [Column(IsQueryAll = true)]
    [Category("GET,POST")]
    [DisplayName("HTTP请求")]
    public HttpMethod HttpMethod { get; set; }

    /// <summary>
    /// 取得或设置方法信息。
    /// </summary>
    [JsonIgnore]
    public MethodInfo MethodInfo { get; set; }

    /// <summary>
    /// 取得或设置方法参数集合。
    /// </summary>
    [JsonIgnore]
    public ParameterInfo[] Parameters { get; set; }
}