﻿namespace Known;

public partial class Language
{
    private static List<ActionInfo> items;

    /// <summary>
    /// 取得多语言项目列表（简体中文/繁体中文/English/Việt Nam）。
    /// </summary>
    public static List<ActionInfo> Items
    {
        get
        {
            items ??= [
                new ActionInfo { Id = "zh-CN", Name = "简体中文", Icon = "简" },
                new ActionInfo { Id = "zh-TW", Name = "繁体中文", Icon = "繁" },
                new ActionInfo { Id = "en-US", Name = "English", Icon = "EN" },
                new ActionInfo { Id = "vi-VN", Name = "Việt Nam", Icon = "VN" }
            ];
            return items;
        }
    }

    /// <summary>
    /// 根据语言标识获取语言项目。
    /// </summary>
    /// <param name="name">语言标识</param>
    /// <returns>语言项目对象。</returns>
    public static ActionInfo GetLanguage(string name)
    {
        if (string.IsNullOrWhiteSpace(name))
            name = CultureInfo.CurrentCulture.Name;

        var info = Items?.FirstOrDefault(l => l.Id == name);
        info ??= Items?.FirstOrDefault();
        return info;
    }

    internal static void Initialize(Assembly assembly)
    {
        foreach (var item in Items)
        {
            var content = Utils.GetResource(assembly, $"Locales.{item.Id}");
            if (string.IsNullOrWhiteSpace(content))
                continue;

            if (!caches.ContainsKey(item.Id))
                caches[item.Id] = [];

            var langs = Utils.FromJson<Dictionary<string, object>>(content);
            if (langs != null && langs.Count > 0)
            {
                foreach (var lang in langs)
                {
                    caches[item.Id][lang.Key] = lang.Value;
                }
            }
        }
    }

    /// <summary>
    /// 获取菜单语言。
    /// </summary>
    /// <param name="info">菜单信息对象。</param>
    /// <returns>菜单语言。</returns>
    public string GetString(MenuInfo info) => GetText("Menu", info?.Code, info?.Name);

    /// <summary>
    /// 获取操作按钮语言。
    /// </summary>
    /// <param name="info">操作按钮对象。</param>
    /// <returns>操作按钮语言。</returns>
    public string GetString(ActionInfo info) => GetText("Button", info?.Id, info?.Name);

    /// <summary>
    /// 获取字段语言。
    /// </summary>
    /// <param name="info">字段信息对象。</param>
    /// <param name="type">数据类型。</param>
    /// <returns>字段语言。</returns>
    public string GetString(ColumnInfo info, Type type = null) => GetText(type?.Name, info?.Id, info?.Name);

    /// <summary>
    /// 获取指定类型的字段语言。
    /// </summary>
    /// <typeparam name="T">数据类型。</typeparam>
    /// <param name="info">字段信息对象。</param>
    /// <returns>字段语言。</returns>
    public string GetString<T>(ColumnInfo info) => GetText(typeof(T).Name, info?.Id, info?.Name);
}