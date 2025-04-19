namespace Known;

public partial class ColumnInfo
{
    /// <summary>
    /// 取得或设置栏位是否是合并行字段。
    /// </summary>
    public bool IsMergeRow { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是合并列字段。
    /// </summary>
    public bool IsMergeColumn { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是汇总字段。
    /// </summary>
    public bool IsSum { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是排序字段。
    /// </summary>
    public bool IsSort { get; set; }

    /// <summary>
    /// 取得或设置栏位默认排序方法（升序/降序）。
    /// </summary>
    public string DefaultSort { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查看连接（设为True，才可在线配置表单，为False，则默认为普通查询表格）。
    /// </summary>
    public bool IsViewLink { get; set; }

    /// <summary>
    /// 取得或设置栏位是否是查询条件。
    /// </summary>
    public bool IsQuery { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件下拉框是否显示【全部】。
    /// </summary>
    public bool IsQueryAll { get; set; }

    /// <summary>
    /// 取得或设置栏位查询条件默认值。
    /// </summary>
    public string QueryValue { get; set; }

    /// <summary>
    /// 取得或设置栏位固定列位置（left/right）。
    /// </summary>
    public string Fixed { get; set; }

    /// <summary>
    /// 取得或设置栏位宽度。
    /// </summary>
    public int? Width { get; set; }

    /// <summary>
    /// 取得或设置栏位显示顺序。
    /// </summary>
    public int Sort { get; set; }

    /// <summary>
    /// 取得或设置栏位对齐方式（left/center/right）。
    /// </summary>
    public string Align { get; set; }

    /// <summary>
    /// 取得或设置栏位默认显示位置。
    /// </summary>
    public int? Position { get; set; }

    /// <summary>
    /// 获取查询条件默认值。
    /// </summary>
    /// <param name="defaultValue">默认值对象。</param>
    /// <param name="user">当前用户。</param>
    /// <returns></returns>
    public string GetDefaultValue(object defaultValue, UserInfo user)
    {
        if (defaultValue == null)
            return DataPlaceholder.FormatValue(QueryValue, user)?.ToString();

        if (defaultValue is Dictionary<string, object>)
        {
            (defaultValue as Dictionary<string, object>).TryGetValue(Id, out object value);
            return value?.ToString();
        }

        return TypeHelper.GetPropertyValue<string>(defaultValue, Id);
    }
}