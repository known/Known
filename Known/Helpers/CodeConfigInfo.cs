namespace Known.Helpers;

/// <summary>
/// 代码配置信息类。
/// </summary>
public class CodeConfigInfo
{
    /// <summary>
    /// 取得或设置前实体类路径。
    /// </summary>
    public string EntityPath { get; set; }

    /// <summary>
    /// 取得或设置前信息类路径。
    /// </summary>
    public string ModelPath { get; set; }

    /// <summary>
    /// 取得或设置前端页面路径。
    /// </summary>
    public string PagePath { get; set; }

    /// <summary>
    /// 取得或设置前端表单路径。
    /// </summary>
    public string FormPath { get; set; }

    /// <summary>
    /// 取得或设置服务接口路径。
    /// </summary>
    public string ServiceIPath { get; set; }

    /// <summary>
    /// 取得或设置服务实现类路径。
    /// </summary>
    public string ServicePath { get; set; }
}