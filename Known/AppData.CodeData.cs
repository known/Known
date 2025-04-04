namespace Known;

public partial class AppData
{
    // 代码生成模型文件路径
    internal static string KcdPath { get; set; }

    internal static List<CodeModelInfo> LoadCodeModels()
    {
        if (!File.Exists(KcdPath))
            return [];

        var bytes = File.ReadAllBytes(KcdPath);
        return ParseData<List<CodeModelInfo>>(bytes);
    }

    internal static void SaveCodeModel(CodeModelInfo info)
    {
        var models = LoadCodeModels();
        var model = models.FirstOrDefault(m => m.Id == info.Id);
        if (model != null)
        {
            model.Code = info.Code;
            model.Name = info.Name;
            model.Prefix = info.Prefix;
            model.Namespace = info.Namespace;
            model.Functions = info.Functions;
            model.Fields = info.Fields;
        }
        else
        {
            models.Add(info);
        }
        var bytes = FormatData(models);
        File.WriteAllBytes(KcdPath, bytes);
    }
}