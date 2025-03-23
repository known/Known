namespace Known;

public partial class Config
{
    private static void InitAssembly(Assembly assembly)
    {
        if (assembly == null)
            return;

        if (InitAssemblies.Contains(assembly.FullName))
            return;

        InitAssemblies.Add(assembly.FullName);

        foreach (var item in assembly.GetTypes())
        {
            if (item.IsInterface && !item.IsGenericTypeDefinition && item.IsAssignableTo(typeof(IService)) && item.Name != nameof(IService))
                AddApiMethod(item, item.Name[1..].Replace("Service", ""));
            else if (item.IsEnum)
                Cache.AttachEnumCodes(item);

            AddCodeInfo(item);
        }
    }
}