namespace Known.Razor.Components;

class FormContext : FieldContext
{
    internal dynamic Data
    {
        get
        {
            var obj = new ExpandoObject();
            foreach (var item in Fields)
            {
                obj.TryAdd(item.Key, item.Value.GetFieldValue());
            }
            return obj;
        }
    }

    internal Dictionary<string, IBrowserFile> Files
    {
        get
        {
            var dic = new Dictionary<string, IBrowserFile>();
            foreach (var item in Fields)
            {
                if (item.Value is Upload)
                {
                    var upload = item.Value as Upload;
                    if (upload.File != null)
                        dic.TryAdd(item.Key, upload.File);
                }
            }
            return dic;
        }
    }

    internal Dictionary<string, List<IBrowserFile>> MultiFiles
    {
        get
        {
            var dic = new Dictionary<string, List<IBrowserFile>>();
            foreach (var item in Fields)
            {
                if (item.Value is Upload)
                {
                    var upload = item.Value as Upload;
                    if (upload.Files != null && upload.Files.Count > 0)
                        dic.TryAdd(item.Key, upload.Files);
                }
            }
            return dic;
        }
    }

    internal bool Validate()
    {
        var errors = new List<string>();
        foreach (var item in Fields)
        {
            if (!item.Value.Validate())
                errors.Add(item.Key);
        }

        return errors.Count == 0;
    }

    internal bool ValidateCheck(bool isPass)
    {
        var errors = new List<string>();
        foreach (var item in Fields)
        {
            if (!item.Value.Validate())
                errors.Add(item.Key);
        }

        return errors.Count == 0;
    }

    internal void Clear()
    {
        foreach (var item in Fields)
        {
            item.Value.ClearFieldValue();
        }
    }

    internal void SetData(object data)
    {
        var model = Utils.MapTo<Dictionary<string, object>>(data);
        foreach (var item in Fields)
        {
            var value = model.ContainsKey(item.Key) ? model[item.Key] : null;
            item.Value.SetFieldValue(value);
        }
    }

    internal void SetReadOnly(bool readOnly)
    {
        foreach (var item in Fields)
        {
            item.Value.SetFieldReadOnly(readOnly);
        }
    }

    internal void SetEnabled(bool enabled)
    {
        foreach (var item in Fields)
        {
            item.Value.SetFieldEnabled(enabled);
        }
    }
}