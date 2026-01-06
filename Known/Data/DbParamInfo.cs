namespace Known.Data;

/// <summary>
/// 数据库命令参数类。
/// </summary>
public class DbParamInfo
{
    /// <summary>
    /// 取得或设置参数名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置参数值。
    /// </summary>
    public object Value { get; set; }

    /// <summary>
    /// 取得或设置参数方向。
    /// </summary>
    public ParameterDirection Direction { get; set; } = ParameterDirection.Input;
}