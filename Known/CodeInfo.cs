namespace Known;

public class CodeInfo
{
    public CodeInfo() { }

    public CodeInfo(string code, string name, object data = null)
    {
        Code = code;
        Name = name;
        Data = data;
    }

    public CodeInfo(string category, string code, string name, object data = null)
    {
        Category = category;
        Code = code;
        Name = name;
        Data = data;
    }

    public string Category { get; set; }
    public string Code { get; set; }
    public string Name { get; set; }
    public bool IsActive { get; set; }
    public object Data { get; set; }

    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    public static List<CodeInfo> GetCodes(string category)
    {
        if (string.IsNullOrEmpty(category))
            return new List<CodeInfo>();

        var codes = Cache.GetCodes(category);
        if (codes != null && codes.Count > 0)
            return codes;

        return category.Split(',', ';').Select(d => new CodeInfo(d, d)).ToList();
    }

    public static CodeInfo[] GetChildCodes(string category, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return null;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == value);
        var dic = code?.Data is SysDictionary
                ? code?.Data as SysDictionary
                : Utils.FromJson<SysDictionary>(code?.Data?.ToString());
        if (!string.IsNullOrWhiteSpace(dic?.Child))
            return dic?.Children.Select(c => new CodeInfo(c.Code, c.Name)).ToArray();

        if (!string.IsNullOrWhiteSpace(dic?.Extension))
        {
            return dic?.Extension.Split(Environment.NewLine.ToArray())
                       .Where(s => !string.IsNullOrWhiteSpace(s))
                       .Select(s => new CodeInfo(s, s))
                       .ToArray();
        }

        return null;
    }

    public static string GetCode(string category, string codeOrName)
    {
        if (string.IsNullOrWhiteSpace(codeOrName))
            return string.Empty;

        var codes = GetCodes(category);
        var code = codes.FirstOrDefault(c => c.Code == codeOrName || c.Name == codeOrName);
        return code?.Code;
    }
}