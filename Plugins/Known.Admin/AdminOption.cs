namespace Known;

public class AdminOption
{
    internal static AdminOption Instance = new();
    internal List<Action<List<SysModule1>>> Modules { get; } = [];

    public CodeConfigInfo Code { get; set; }

    public void AddModules(Action<List<SysModule1>> action)
    {
        Modules.Add(action);
    }
}