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
                obj.TryAdd(item.Key, item.Value.GetValue());
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
        foreach (var field in Fields)
        {
            if (!field.Value.Validate())
                errors.Add(field.Key);
        }

        return errors.Count == 0;
    }

    internal bool ValidateCheck(bool isPass)
    {
        var errors = new List<string>();
        foreach (var field in Fields)
        {
            if (!field.Value.Validate())
                errors.Add(field.Key);
        }

        return errors.Count == 0;
    }

    internal void Clear()
    {
        foreach (var field in Fields)
        {
            field.Value.Clear();
        }
    }

    internal void SetData(object data)
    {
        var model = Utils.MapTo<Dictionary<string, object>>(data);
        foreach (var item in Fields)
        {
            var value = model.ContainsKey(item.Key) ? model[item.Key] : null;
            item.Value.SetValue(value);
        }
    }
}