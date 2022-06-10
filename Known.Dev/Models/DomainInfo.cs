/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System.Linq;

namespace Known.Dev.Models;

public class DomainInfo
{
    public string Id { get; set; }
    public string System { get; set; }
    public string Category { get; set; }
    public string Prefix { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public string FieldData { get; set; }
}

public class FieldInfo
{
    public string Code { get; set; }
    public string Name { get; set; }
    public string Type { get; set; }
    public string Length { get; set; }
    public string Control { get; set; }
    public string Width { get; set; }
    public string Align { get; set; }
    public string Codes { get; set; }
    public int OnlyForm { get; set; }
    public int Required { get; set; }
    public int Query { get; set; }
    public int Sort { get; set; }
    public int Import { get; set; }
    public int Export { get; set; }

    internal void SetDefault()
    {
        if (string.IsNullOrWhiteSpace(Align))
        {
            if (new string[] { "date", "radio", "checkbox" }.Contains(Control))
                Align = "center";
            else if (new string[] { "int", "decimal" }.Contains(Type))
                Align = "right";
        }

        if (string.IsNullOrWhiteSpace(Codes))
        {
            if (Control == "checkbox")
                Codes = "YesNo";
        }
    }
}