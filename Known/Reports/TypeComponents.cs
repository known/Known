namespace Known.Reports;

/// <summary>
/// 数据源类型枚举选择组件类。
/// </summary>
public class SelectDataSourceType : AntSelectEnum<DataSourceType> { }

/// <summary>
/// 图表类型枚举选择组件类。
/// </summary>
public class SelectChartType : AntSelectEnum<ChartType> { }

/// <summary>
/// 聚合函数类型枚举选择组件类。
/// </summary>
public class SelectAggregateType : AntSelectEnum<AggregateType> { }

/// <summary>
/// 报表类型表单组件类。
/// </summary>
public class ReportTypeForm : AntForm<SysReport> { }

/// <summary>
/// 报表块配置表单组件类。
/// </summary>
public class ReportBlockForm : AntForm<ReportBlock> { }

/// <summary>
/// 列配置表格组件类。
/// </summary>
public class ColumnConfigTable : AntTable<ColumnConfig> { }

/// <summary>
/// 实体字段配置表格组件类。
/// </summary>
public class EntityFieldConfigTable : AntTable<EntityFieldConfig> { }