﻿namespace Known.Blazor;

public partial class ColumnBuilder<TItem>
{
    /// <summary>
    /// 设置表格栏位为列头过滤字段。
    /// </summary>
    /// <param name="isFilter">是否显示过滤器。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> Filter(bool isFilter)
    {
        if (column != null)
            column.IsFilter = isFilter;
        return this;
    }

    /// <summary>
    /// 设置表格栏位列头是否显示过滤器查询类型。
    /// </summary>
    /// <param name="isFilterType">是否显示过滤器查询类型。</param>
    /// <returns>表格栏位建造者。</returns>
    public ColumnBuilder<TItem> FilterType(bool isFilterType)
    {
        if (column != null)
            column.IsFilterType = isFilterType;
        return this;
    }
}