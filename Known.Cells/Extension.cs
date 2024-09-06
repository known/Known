using Microsoft.Extensions.DependencyInjection;

namespace Known.Cells;

/// <summary>
/// 依赖注入扩展类。
/// </summary>
public static class Extension
{
    /// <summary>
    /// 添加基于Aspose.Cells实现的Excel操作。
    /// </summary>
    /// <param name="services">服务集合。</param>
    public static void AddKnownCells(this IServiceCollection services)
    {
        ExcelFactory.Factory = new AsposeExcelFactory();
    }
}