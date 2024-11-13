namespace Known.Core.Extensions;

/// <summary>
/// 数据模型扩展类。
/// </summary>
public static class ModelExtension
{
    #region Module
    internal static List<MenuInfo> ToMenus(this List<SysModule> modules, bool isAdmin)
    {
        if (modules == null || modules.Count == 0)
            return [];

        return modules.Select(m => new MenuInfo(m, isAdmin)).ToList();
    }

    internal static void RemoveModule(this List<SysModule> modules, string code)
    {
        var module = modules.FirstOrDefault(m => m.Code == code);
        if (module != null)
            modules.Remove(module);
    }
    #endregion
}