/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

namespace Known.Razor;

public class MenuItem
{
    public MenuItem()
    {
        Children = new List<MenuItem>();
    }

    public MenuItem(string name, Type type) : this()
    {
        Id = type.Name;
        Name = name;
        Type = type;
    }

    public MenuItem(string id, string name, string icon, string description = null) : this()
    {
        Id = id;
        Name = name;
        Icon = icon;
        Description = description;
    }

    public MenuItem(string name, string icon, Type type = null) : this()
    {
        Id = type?.Name;
        Name = name;
        Icon = icon;
        Type = type;
    }

    public string Id { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }
    public string Icon { get; set; }
    public string Title { get; set; }
    public string Color { get; set; }
    public int Badge { get; set; }
    public Type Type { get; set; }
    public Action Action { get; set; }
    public List<MenuItem> Children { get; set; }
}
