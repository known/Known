namespace Known;

public class AdminOption
{
    internal static AdminOption Instance = new();
    internal List<Action<List<SysModule>>> Modules { get; } = [];

    public CodeConfigInfo Code { get; set; }

    public void AddModules(Action<List<SysModule>> action)
    {
        Modules.Add(action);
    }
}