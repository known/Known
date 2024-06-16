namespace Known;

public sealed class Utils
{
    private Utils() { }

    #region Common
    public static string GetGuid() => Guid.NewGuid().ToString("N").ToLower().Replace("-", "");
    public static T ConvertTo<T>(object value, T defaultValue = default) => (T)ConvertTo(typeof(T), value, defaultValue);

    public static object ConvertTo(Type type, object value, object defaultValue = null)
    {
        if (value == null || value == DBNull.Value)
            return defaultValue;

        var valueString = value.ToString();
        if (type == typeof(string))
            return Convert.ChangeType(valueString, type);

        valueString = valueString.Trim();
        if (valueString.Length == 0)
            return defaultValue;

        if (type.IsEnum)
            return Enum.Parse(type, valueString, true);

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = Nullable.GetUnderlyingType(type);

        if (type == typeof(bool) || type == typeof(bool?))
            valueString = ",是,1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

        try
        {
            return Convert.ChangeType(valueString, type);
        }
        catch
        {
            return defaultValue;
        }
    }

    public static bool CheckMobile(string agent)
    {
        if (agent.Contains("Windows NT") || agent.Contains("Macintosh"))
            return false;

        bool flag = false;
        string[] keywords = ["Android", "iPhone", "iPod", "Windows Phone", "MQQBrowser"];

        foreach (string item in keywords)
        {
            if (agent.Contains(item))
            {
                flag = true;
                break;
            }
        }

        return flag;
    }

    public static string GetMaxFormNo(string prefix, string maxNo)
    {
        var lastNo = maxNo.Replace(prefix, "");
        var length = lastNo.Length;
        lastNo = lastNo.TrimStart('0');
        var no = string.IsNullOrWhiteSpace(lastNo) ? 0 : int.Parse(lastNo);
        return string.Format("{0}{1:D" + length + "}", prefix, no + 1);
    }

    public static string GetMaxFormNo(string prefix, string suffix, string maxNo)
    {
        var lastNo = maxNo.Replace(prefix, "").Replace(suffix, "");
        var length = lastNo.Length;
        lastNo = lastNo.TrimStart('0');
        var no = string.IsNullOrWhiteSpace(lastNo) ? 0 : int.Parse(lastNo);
        return string.Format("{0}{1:D" + length + "}{2}", prefix, no + 1, suffix);
    }
    #endregion

    #region Round
    public static decimal Round(decimal value, int decimals) => Math.Round(value, decimals, MidpointRounding.AwayFromZero);
    public static double Round(double value, int decimals) => Math.Round(value, decimals, MidpointRounding.AwayFromZero);

    public static string GetCaptcha(int length)
    {
        var chars = "abcdefghijkmnpqrstuvwxyz2345678ABCDEFGHJKLMNPQRSTUVWXYZ";
        var rnd = new Random();
        var code = "";
        for (int i = 0; i < length; i++)
        {
            code += chars[rnd.Next(chars.Length)];
        }
        return code;
    }
    #endregion

    #region Encryptor
    public static string ToMd5(string value)
    {
        if (string.IsNullOrEmpty(value))
            return string.Empty;

        var buffer = Encoding.UTF8.GetBytes(value);
        var bytes = MD5.HashData(buffer);

        var sb = new StringBuilder();
        foreach (var item in bytes)
        {
            sb.Append(item.ToString("x2"));
        }
        return sb.ToString();
    }

    public static string ToSHA1(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            return string.Empty;

        var buffer = Encoding.UTF8.GetBytes(value);
        var bytes = SHA1.HashData(buffer);

        var sb = new StringBuilder();
        foreach (var item in bytes)
        {
            sb.Append(item.ToString("x2"));
        }
        return sb.ToString();
    }
    #endregion

    #region Serialize
    public static string ToJson(object value)
    {
        if (value == null)
            return string.Empty;

        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    public static dynamic ToDynamic(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default;

        var dics = FromJson<Dictionary<string, object>>(json);
        var obj = new ExpandoObject();
        foreach (var item in dics)
        {
            obj.TryAdd(item.Key, item.Value?.ToString());
        }

        return obj;
    }

    private static readonly JsonSerializerOptions dsOptions = new()
    {
        PropertyNameCaseInsensitive = true,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private static string FormatDSJson(string json) => json.Replace("{}", "null").Replace("\"\"", "null");

    public static T FromJson<T>(string json)
    {
        if (string.IsNullOrEmpty(json))
            return default;

        try
        {
            json = FormatDSJson(json);
            return JsonSerializer.Deserialize<T>(json, dsOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{typeof(T).FullName}{Environment.NewLine}{json}{Environment.NewLine}{ex}");
            return default;
        }
    }

    public static object FromJson(Type type, string json)
    {
        if (string.IsNullOrEmpty(json))
            return null;

        try
        {
            json = FormatDSJson(json);
            return JsonSerializer.Deserialize(json, type, dsOptions);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"{type.FullName}{Environment.NewLine}{json}{Environment.NewLine}{ex}");
            return default;
        }
    }

    public static T MapTo<T>(object value)
    {
        if (value == null)
            return default;

        if (value.GetType() == typeof(T))
            return (T)value;

        var json = ToJson(value);
        return FromJson<T>(json);
    }
    #endregion

    #region Resource
    public static string GetResource(Assembly assembly, string name)
    {
        var text = string.Empty;
        if (assembly == null || string.IsNullOrEmpty(name))
            return text;

        var names = assembly.GetManifestResourceNames();
        name = names.FirstOrDefault(n => n.Contains(name));
        if (string.IsNullOrEmpty(name))
            return text;

        var stream = assembly.GetManifestResourceStream(name);
        if (stream != null)
        {
            using var sr = new StreamReader(stream);
            text = sr.ReadToEnd();
        }
        return text;
    }

    public static string GetFileName(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return string.Empty;

        var file = new FileInfo(fileName);
        return file.Name;
    }

    public static void EnsureFile(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return;

        var file = new FileInfo(fileName);
        if (!file.Directory.Exists)
        {
            file.Directory.Create();
        }
    }

    public static void CopyFile(string sourceFileName, string destFileName, bool overwrite = true)
    {
        var info = new FileInfo(destFileName);
        if (!info.Directory.Exists)
            info.Directory.Create();

        File.Copy(sourceFileName, destFileName, overwrite);
    }

    public static string ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        if (!File.Exists(path))
            return string.Empty;

        return File.ReadAllText(path);
    }

    public static void SaveFile(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (string.IsNullOrEmpty(content))
            return;

        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        File.WriteAllText(path, content);
    }

    public static void SaveFile(string path, byte[] bytes)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (bytes == null || bytes.Length == 0)
            return;

        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        File.WriteAllBytes(path, bytes);
    }

    public static void DeleteFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!File.Exists(path))
            return;

        File.Delete(path);
    }
    #endregion

    #region Color
    public static Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
    }

    public static Color FromHtml(string htmlColor) => ColorTranslator.FromHtml(htmlColor);
    public static string ToHtml(Color color) => ColorTranslator.ToHtml(color);
    #endregion

    #region Network
    public static bool Ping(string host, int timeout = 120)
    {
        try
        {
            var ping = new Ping();
            var options = new PingOptions { DontFragment = true };
            var data = "";
            var buffer = Encoding.UTF8.GetBytes(data);
            var reply = ping.Send(host, timeout, buffer, options);
            return reply.Status == IPStatus.Success;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public static bool HasNetwork() => Ping("www.baidu.com");
    #endregion

    #region Byte
    public static byte[] StreamToBytes(Stream stream)
    {
        if (stream == null)
            return null;

        var bytes = new byte[stream.Length];
        stream.Read(bytes, 0, bytes.Length);
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    public static byte[] HexToBytes(string hexString)
    {
        if (string.IsNullOrEmpty(hexString))
            return null;

        hexString = hexString.Replace(" ", "");
        if ((hexString.Length % 2) != 0)
            hexString += " ";

        var bytes = new byte[hexString.Length / 2];
        for (int i = 0; i < bytes.Length; i++)
        {
            bytes[i] = Convert.ToByte(hexString.Substring(i * 2, 2).Trim(), 16);
        }

        return bytes;
    }

    public static string BytesToHex(byte[] bytes, string separator = " ")
    {
        if (bytes == null || bytes.Length == 0)
            return string.Empty;

        var items = new List<string>();
        for (int i = 0; i < bytes.Length; i++)
        {
            items.Add(((int)bytes[i]).ToString("X2"));
        }

        return string.Join(separator, items.ToArray());
    }
    #endregion

    #region Pinyin
    public static string GetPinyin(string value)
    {
        string temp = "";
        int iLen = value.Length;
        for (var i = 0; i <= iLen - 1; i++)
        {
            temp += GetCharSpellCode(value.Substring(i, 1));
        }
        return temp;
    }

    private static string GetCharSpellCode(string value)
    {
        long iCnChar;
        byte[] ZW = Encoding.Default.GetBytes(value);

        if (ZW.Length == 1)
            return value.ToUpper();

        int i1 = ZW[0];
        int i2 = ZW[1];
        iCnChar = i1 * 256 + i2;

        if ((iCnChar >= 45217) && (iCnChar <= 45252))
            return "A";
        else if ((iCnChar >= 45253) && (iCnChar <= 45760))
            return "B";
        else if ((iCnChar >= 45761) && (iCnChar <= 46317))
            return "C";
        else if ((iCnChar >= 46318) && (iCnChar <= 46825))
            return "D";
        else if ((iCnChar >= 46826) && (iCnChar <= 47009))
            return "E";
        else if ((iCnChar >= 47010) && (iCnChar <= 47296))
            return "F";
        else if ((iCnChar >= 47297) && (iCnChar <= 47613))
            return "G";
        else if ((iCnChar >= 47614) && (iCnChar <= 48118))
            return "H";
        else if ((iCnChar >= 48119) && (iCnChar <= 49061))
            return "J";
        else if ((iCnChar >= 49062) && (iCnChar <= 49323))
            return "K";
        else if ((iCnChar >= 49324) && (iCnChar <= 49895))
            return "L";
        else if ((iCnChar >= 49896) && (iCnChar <= 50370))
            return "M";
        else if ((iCnChar >= 50371) && (iCnChar <= 50613))
            return "N";
        else if ((iCnChar >= 50614) && (iCnChar <= 50621))
            return "O";
        else if ((iCnChar >= 50622) && (iCnChar <= 50905))
            return "P";
        else if ((iCnChar >= 50906) && (iCnChar <= 51386))
            return "Q";
        else if ((iCnChar >= 51387) && (iCnChar <= 51445))
            return "R";
        else if ((iCnChar >= 51446) && (iCnChar <= 52217))
            return "S";
        else if ((iCnChar >= 52218) && (iCnChar <= 52697))
            return "T";
        else if ((iCnChar >= 52698) && (iCnChar <= 52979))
            return "W";
        else if ((iCnChar >= 52980) && (iCnChar <= 53640))
            return "X";
        else if ((iCnChar >= 53689) && (iCnChar <= 54480))
            return "Y";
        else if ((iCnChar >= 54481) && (iCnChar <= 55289))
            return "Z";

        return "?";
    }
    #endregion
}