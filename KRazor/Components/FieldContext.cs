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

using System.Dynamic;

namespace Known.Razor;

public class FieldContext
{
    public FieldContext()
    {
        Fields = new Dictionary<string, Field>();
    }

    public bool ReadOnly { get; set; }
    public bool IsTableForm { get; set; }
    public bool IsGridView { get; set; }
    public string Field { get; set; }
    public Dictionary<string, object> Model { get; set; }
    public Dictionary<string, Field> Fields { get; }

    public List<Field> DataFields
    {
        get
        {
            if (Fields == null || Fields.Count == 0)
                return null;

            return Fields.Values.ToList();
        }
    }

    public List<Field> QueryFields
    {
        get
        {
            if (Fields == null || Fields.Count == 0)
                return null;

            return Fields.Values.Where(c => c.IsQuery).ToList();
        }
    }

    public dynamic Data
    {
        get
        {
            var obj = new ExpandoObject();
            foreach (var item in Fields)
            {
                obj.TryAdd(item.Key, item.Value.GetValue());
            }
            return obj;
        }
    }

    public bool Validate()
    {
        var errors = new List<string>();
        foreach (var field in Fields)
        {
            if (!field.Value.Validate())
                errors.Add(field.Key);
        }

        return errors.Count == 0;
    }

    public void Clear()
    {
        foreach (var field in Fields)
        {
            field.Value.Clear();
        }
    }

    public Dictionary<string, string> GetData()
    {
        var dic = new Dictionary<string, string>();
        foreach (var item in Fields)
        {
            dic.TryAdd(item.Key, item.Value.Value);
        }
        return dic;
    }

    public void SetData(object data)
    {
        var model = Utils.MapTo<Dictionary<string, object>>(data);
        foreach (var item in Fields)
        {
            var value = model.ContainsKey(item.Key) ? model[item.Key] : null;
            item.Value.SetValue(value);
        }
    }
}
