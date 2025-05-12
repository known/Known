namespace Known.Blazor;

partial class TableModel<TItem>
{
    /// <summary>
    /// 获取表格栏位建造者对象。
    /// </summary>
    /// <typeparam name="TValue">栏位属性类型。</typeparam>
    /// <param name="selector">栏位属性选择表达式。</param>
    /// <returns>栏位建造者对象。</returns>
    public ColumnBuilder<TItem> Column<TValue>(Expression<Func<TItem, TValue>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = Columns?.FirstOrDefault(c => c.Id == property.Name);
        var allColumn = AllColumns?.FirstOrDefault(c => c.Id == property.Name);
        return new ColumnBuilder<TItem>(column, allColumn, this);
    }

    /// <summary>
    /// 添加一个表格栏位。
    /// </summary>
    /// <typeparam name="TValue">栏位属性类型。</typeparam>
    /// <param name="selector">栏位属性选择表达式。</param>
    /// <param name="isQuery">是否是查询字段。</param>
    /// <returns>栏位建造者对象。</returns>
    public ColumnBuilder<TItem> AddColumn<TValue>(Expression<Func<TItem, TValue>> selector, bool isQuery = false)
    {
        var property = TypeHelper.Property(selector);
        return AddColumn(property, isQuery);
    }

    /// <summary>
    /// 添加一个表格栏位。
    /// </summary>
    /// <param name="property">栏位属性。</param>
    /// <param name="isQuery">是否是查询字段。</param>
    /// <returns>栏位建造者对象。</returns>
    public ColumnBuilder<TItem> AddColumn(PropertyInfo property, bool isQuery = false)
    {
        if (AllColumns.Exists(c => c.Id == property.Name))
        {
            var column = Columns.FirstOrDefault(c => c.Id == property.Name);
            var allColumn = AllColumns.FirstOrDefault(c => c.Id == property.Name);
            return new ColumnBuilder<TItem>(column, allColumn, this);
        }
        else
        {
            var column = new ColumnInfo(property) { IsQuery = isQuery };
            if (property.DeclaringType != typeof(TItem))
                column.Id = $"{property.DeclaringType.Name}.{property.Name}";
            AllColumns.Add(column);
            Columns.Add(column);
            if (isQuery)
                AddQueryColumn(column);
            return new ColumnBuilder<TItem>(column, column, this);
        }
    }

    /// <summary>
    /// 添加额外查询条件字段。
    /// </summary>
    /// <param name="selector">栏位属性选择表达式。</param>
    public void AddQueryColumn(Expression<Func<TItem, object>> selector)
    {
        var property = TypeHelper.Property(selector);
        var column = new ColumnInfo(property);
        AddQueryColumn(column);
    }

    /// <summary>
    /// 添加额外查询条件字段。
    /// </summary>
    /// <param name="column">栏位信息。</param>
    public void AddQueryColumn(ColumnInfo column)
    {
        if (QueryColumns.Exists(c => c.Id == column.Id))
            return;

        QueryColumns.Add(column);
        QueryData[column.Id] = new QueryInfo(column);
    }

    /// <summary>
    /// 添加额外查询条件字段。
    /// </summary>
    /// <param name="id">字段ID。</param>
    /// <param name="name">字段名称。</param>
    /// <param name="type">查询类型。</param>
    /// <param name="value">默认值。</param>
    public void AddQueryColumn(string id, string name, QueryType type = QueryType.Contain, string value = "")
    {
        if (QueryColumns.Exists(c => c.Id == id))
            return;

        var column = new ColumnInfo { Id = id, Name = name };
        QueryColumns.Add(column);
        QueryData[column.Id] = new QueryInfo(id, type, value);
    }

    internal bool SetAutoColumns(List<TItem> dataSource)
    {
        if (Columns != null && Columns.Count > 0) return false;
        if (!IsDictionary) return false;
        if (dataSource == null || dataSource.Count == 0) return false;

        var dic = dataSource as List<Dictionary<string, object>>;
        var columns = dic.FirstOrDefault().GetColumns();
        if (columns.Count > 0)
            Columns.AddRange(columns);
        return true;
    }

    internal List<ColumnInfo> GetUserColumns()
    {
        Context.UserTableSettings.TryGetValue(SettingId, out List<TableSettingInfo> settings);
        var infos = new List<ColumnInfo>();
        if (AllColumns != null && AllColumns.Count > 0)
        {
            foreach (var item in AllColumns)
            {
                var info = item.Clone();
                var setting = settings?.FirstOrDefault(c => c.Id == item.Id);
                if (setting != null)
                {
                    info.Fixed = setting.Fixed;
                    info.IsVisible = setting.IsVisible;
                    info.Width = setting.Width;
                    info.Sort = setting.Sort;
                }
                infos.Add(info);
            }
        }
        return [.. infos.OrderBy(c => c.Sort)];
    }
}