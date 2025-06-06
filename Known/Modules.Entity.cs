﻿namespace Known;

/// <summary>
/// 在线实体模型配置信息类。
/// </summary>
public class EntityInfo
{
    /// <summary>
    /// 取得或设置实体ID。
    /// </summary>
    public string Id { get; set; }

    /// <summary>
    /// 取得或设置实体名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置实体对应的页面URL。
    /// </summary>
    public string PageUrl { get; set; }

    /// <summary>
    /// 取得或设置数据库表名。
    /// </summary>
    public string TableName { get; set; }

    /// <summary>
    /// 取得或设置是否继承实体基类，默认是。
    /// </summary>
    public bool IsEntity { get; set; } = true;

    /// <summary>
    /// 取得或设置是否是工作流实体。
    /// </summary>
    public bool IsFlow { get; set; }

    /// <summary>
    /// 取得或设置实体字段信息列表。
    /// </summary>
    public List<FieldInfo> Fields { get; set; } = [];

    /// <summary>
    /// 取得或设置实体字段代码生成配置信息列表。
    /// </summary>
    [JsonIgnore] public List<CodeFieldInfo> CodeFields { get; set; } = [];
}