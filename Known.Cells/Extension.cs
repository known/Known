using Microsoft.Extensions.DependencyInjection;

namespace Known.Cells;

public static class Extension
{
    public static void AddKnownCells(this IServiceCollection services)
    {
        ExcelFactory.Factory = new AsposeExcelFactory();
    }
}