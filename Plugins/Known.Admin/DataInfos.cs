namespace Known;

/// <summary>
/// 设置信息类。
/// </summary>
public class SettingInfo
{
    /// <summary>
    /// 构造函数，创建一个设置信息类的实例。
    /// </summary>
    public SettingInfo()
    {
        Id = Utils.GetNextId();
    }

    /// <summary>
    /// 取得或设置ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置创建人。
    /// </summary>
    public string CreateBy { get; set; }

    /// <summary>
    /// 取得或设置业务类型。
    /// </summary>
    public string BizType { get; set; }

    /// <summary>
    /// 取得或设置业务名称。
    /// </summary>
    public string BizName { get; set; }

    /// <summary>
    /// 取得或设置业务数据。
    /// </summary>
    public string BizData { get; set; }

    /// <summary>
    /// 将业务数据JSON转换成泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>() => Utils.FromJson<T>(BizData);
}