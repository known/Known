using AntDesign;

namespace Known.Helpers;

class IconHelper
{
    public static void LoadAntIcon()
    {
        var antIcons = new List<IconMetaInfo>
        {
            GetAntIconMeta("Outline", typeof(IconType.Outline)),
            GetAntIconMeta("Fill", typeof(IconType.Fill)),
            GetAntIconMeta("TwoTone", typeof(IconType.TwoTone))
        };
        UIConfig.Icons["AntDesign"] = antIcons;
    }

    public static void LoadFAIcon()
    {
        var content = Utils.GetResource(typeof(Extension).Assembly, "IconFA");
        if (string.IsNullOrWhiteSpace(content))
            return;

        var lines = content.Split([.. Environment.NewLine]);
        UIConfig.Icons["FontAwesome"] = ParseFAYaml(lines);
    }

    private static IconMetaInfo GetAntIconMeta(string name, Type type)
    {
        return new IconMetaInfo
        {
            Name = name,
            Type = name,
            Icons = [.. type.GetProperties().Select(x => (string)x.GetValue(null)).Where(x => x is not null)]
        };
    }

    private static List<IconMetaInfo> ParseFAYaml(string[] lines)
    {
        var infos = new List<IconMetaInfo>();
        IconMetaInfo info = null;
        bool inIconsList = false;

        foreach (var line in lines)
        {
            // 跳过空行和注释
            if (string.IsNullOrWhiteSpace(line) || line.TrimStart().StartsWith("#"))
                continue;
            // 检测新分类
            if (!line.StartsWith(" ") && line.EndsWith(":"))
            {
                inIconsList = false;
                info = new IconMetaInfo { Name = line.TrimEnd(':') };
                infos.Add(info);
                continue;
            }
            // 检测 icons 列表
            if (line.Trim().StartsWith("icons:"))
            {
                inIconsList = true;
                continue;
            }
            // 检测 label
            if (line.Trim().StartsWith("label:") && info != null)
            {
                inIconsList = false;
                info.Type = line.Split(':')[1].Trim().Trim('"', '\'');
                continue;
            }
            // 处理 icons 列表项
            if (inIconsList && line.Trim().StartsWith("-") && info != null)
            {
                var iconName = line.Trim().Substring(1).Trim().Trim('"', '\'');
                info.Icons.Add(iconName);
            }
        }
        return infos;
    }
}