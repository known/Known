using System.Security.Cryptography;
using System.Xml.Serialization;

namespace Known;

/// <summary>
/// 系统效用类。
/// </summary>
public sealed class Utils
{
    private static readonly Snowflake snowflake = new(1, 1);
    private Utils() { }

    #region Common
    internal static T GetNextId<T>()
    {
        var id = GetNextId();
        return ConvertTo<T>(id);
    }

    /// <summary>
    /// 根据AppInfo.NextIdType配置获取一个新的ID，默认是GUID。
    /// </summary>
    /// <returns></returns>
    public static string GetNextId()
    {
        return Config.App.NextIdType switch
        {
            NextIdType.Guid => GetGuid(),
            NextIdType.Snowflake => GetSnowflakeId().ToString(),
            //NextIdType.AutoInteger => "-1",
            _ => GetGuid(),
        };
    }

    /// <summary>
    /// 获取一个GUID字符串。
    /// </summary>
    /// <returns></returns>
    public static string GetGuid() => Guid.NewGuid().ToString("N").ToLower().Replace("-", "");

    /// <summary>
    /// 获取一个Snowflake算法的ID。
    /// </summary>
    /// <returns></returns>
    public static long GetSnowflakeId() => snowflake.NextId();

    /// <summary>
    /// 将C#对象转换成指定泛型的对象，适用值类型和String。
    /// </summary>
    /// <typeparam name="T">转换目标类型。</typeparam>
    /// <param name="value">被转换的对象值。</param>
    /// <param name="defaultValue">转换失败默认值。</param>
    /// <returns></returns>
    public static T ConvertTo<T>(object value, T defaultValue = default) => (T)ConvertTo(typeof(T), value, defaultValue);

    /// <summary>
    /// 将C#对象转换成指定类型的对象，适用值类型和String。
    /// </summary>
    /// <param name="type">转换目标类型。</param>
    /// <param name="value">被转换的对象值。</param>
    /// <param name="defaultValue">转换失败默认值。</param>
    /// <returns></returns>
    public static object ConvertTo(Type type, object value, object defaultValue = null)
    {
        if (value == null || value == DBNull.Value)
        {
            if (type == typeof(bool)) return false;
            else if (type == typeof(int)) return 0;
            else if (type == typeof(long)) return 0L;
            else if (type == typeof(decimal)) return 0M;
            else if (type == typeof(double)) return 0D;
            else return defaultValue;
        }

        if (type.IsInstanceOfType(value)) return value;

        var valueString = value.ToString();
        if (type == typeof(string)) return valueString;

        valueString = valueString.Trim();
        if (valueString.Length == 0) return defaultValue;

        if (type.IsEnum)
        {
            if (Enum.TryParse(type, valueString, true, out object enumValue)) return enumValue;
            if (defaultValue != null && Enum.IsDefined(type, defaultValue)) return defaultValue;
            return Enum.GetValues(type).GetValue(0);
        }

        if (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Nullable<>))
            type = Nullable.GetUnderlyingType(type);

        if (type == typeof(bool) || type == typeof(bool?))
            valueString = ",是,1,Y,YES,TRUE,".Contains(valueString.ToUpper()) ? "True" : "False";

        if (type == typeof(TimeOnly) || type == typeof(TimeOnly?))
        {
            if (TimeOnly.TryParse(valueString, out var time)) return time;
            return defaultValue;
        }

        try
        {
            return Convert.ChangeType(valueString, type);
        }
        catch
        {
            return defaultValue;
        }
    }

    /// <summary>
    /// 根据浏览器代理字符串判断是否是移动端请求。
    /// </summary>
    /// <param name="agent">浏览器代理字符串。</param>
    /// <returns></returns>
    public static bool CheckMobile(string agent)
    {
        if (agent.Contains("Windows NT") || agent.Contains("Macintosh"))
            return false;

        bool flag = false;
        string[] keywords = ["Android", "Phone", "iPhone", "iPod", "Windows Phone", "MQQBrowser"];

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

    /// <summary>
    /// 判断文件名是否是图片文件，可检测（jpeg/jpg/png/gif/bmp）。
    /// </summary>
    /// <param name="fileName">文件名。</param>
    /// <returns></returns>
    public static bool CheckImage(string fileName)
    {
        if (string.IsNullOrWhiteSpace(fileName))
            return false;

        return fileName.EndsWith(".jpeg", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".jpg", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".gif", StringComparison.OrdinalIgnoreCase)
            || fileName.EndsWith(".bmp", StringComparison.OrdinalIgnoreCase);
    }

    /// <summary>
    /// 获取指定格式字符串的日期时间类型。
    /// </summary>
    /// <param name="time">日期时间字符串。</param>
    /// <param name="format">日期时间格式字符串，例如：yyyy-MM-dd HH:mm:ss。</param>
    /// <returns></returns>
    public static DateTime? GetDateTime(string time, string format)
    {
        if (string.IsNullOrWhiteSpace(time) || time.StartsWith("0000-00-00"))
            return null;

        if (DateTime.TryParseExact(time, format, CultureInfo.InvariantCulture, DateTimeStyles.None, out DateTime result))
            return result;

        return null;
    }

    /// <summary>
    /// 根据前缀和最大编号获取一个新的最大编号，适用于表单自动编号。
    /// </summary>
    /// <param name="prefix">编号前缀字符串。</param>
    /// <param name="maxNo">当前最大编号。</param>
    /// <returns>最新最大编号。</returns>
    public static string GetMaxFormNo(string prefix, string maxNo)
    {
        var lastNo = maxNo.Replace(prefix, "");
        var length = lastNo.Length;
        lastNo = lastNo.TrimStart('0');
        var no = string.IsNullOrWhiteSpace(lastNo) ? 0 : int.Parse(lastNo);
        return string.Format("{0}{1:D" + length + "}", prefix, no + 1);
    }

    /// <summary>
    /// 根据前缀、后缀和最大编号获取一个新的最大编号，适用于表单自动编号。
    /// </summary>
    /// <param name="prefix">编号前缀字符串。</param>
    /// <param name="suffix">编号后缀字符串。</param>
    /// <param name="maxNo">当前最大编号。</param>
    /// <returns>最新最大编号。</returns>
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
    /// <summary>
    /// 获取Decimal类型的四舍五入值。
    /// </summary>
    /// <param name="value">数值。</param>
    /// <param name="decimals">保留小数位数。</param>
    /// <returns></returns>
    public static decimal Round(decimal? value, int decimals) => Math.Round(value ?? 0, decimals, MidpointRounding.AwayFromZero);

    /// <summary>
    /// 获取Double类型的四舍五入值。
    /// </summary>
    /// <param name="value">数值。</param>
    /// <param name="decimals">保留小数位数。</param>
    /// <returns></returns>
    public static double Round(double? value, int decimals) => Math.Round(value ?? 0, decimals, MidpointRounding.AwayFromZero);

    /// <summary>
    /// 获取随机验证码字符串，大小写英文字幕加数字。
    /// </summary>
    /// <param name="length">字符串长度。</param>
    /// <returns></returns>
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
    /// <summary>
    /// 获取字符串的MD5加密字符串。
    /// </summary>
    /// <param name="value">原字符串。</param>
    /// <returns>MD5加密字符串。</returns>
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

    /// <summary>
    /// 获取字符串的SHA1加密字符串。
    /// </summary>
    /// <param name="value">原字符串。</param>
    /// <returns>SHA1加密字符串。</returns>
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

    /// <summary>
    /// 加密一个字符串。
    /// </summary>
    /// <param name="plainText">原字符串。</param>
    /// <param name="password">密码。</param>
    /// <returns>加密字符串。</returns>
    public static string Encrypt(string plainText, string password = null)
    {
        if (string.IsNullOrWhiteSpace(plainText))
            return string.Empty;

        var buffer = Encoding.UTF8.GetBytes(plainText);
        var pwd = GetPassword(password);
        if (pwd <= 0)
            return Convert.ToBase64String(buffer);

        var bytes = buffer.Select(b => (byte)(b + pwd)).ToArray();
        return Convert.ToBase64String(bytes);
    }

    /// <summary>
    /// 解密一个字符串。
    /// </summary>
    /// <param name="cipherText">加密字符串。</param>
    /// <param name="password">密码。</param>
    /// <returns>原字符串。</returns>
    public static string Decrypt(string cipherText, string password = null)
    {
        if (string.IsNullOrWhiteSpace(cipherText))
            return string.Empty;

        var buffer = Convert.FromBase64String(cipherText);
        var pwd = GetPassword(password);
        if (pwd <= 0)
            return Encoding.UTF8.GetString(buffer);

        var bytes = buffer.Select(b => (byte)(b - pwd)).ToArray();
        return Encoding.UTF8.GetString(bytes);
    }

    private static int GetPassword(string password)
    {
        if (string.IsNullOrWhiteSpace(password))
            return 0;

        var bytes = Encoding.UTF8.GetBytes(password);
        return bytes.Sum(b => b);
    }
    #endregion

    #region Serialize
    /// <summary>
    /// 将对象序列化为XML字符串（使用.NET内置XML序列化）。
    /// </summary>
    /// <param name="value">对象。</param>
    /// <returns>XML字符串。</returns>
    public static string ToXml(object value)
    {
        if (value == null)
            return null;

        var settings = new XmlWriterSettings { Indent = true, Encoding = new UTF8Encoding(false) };
        using var stream = new MemoryStream();
        using var writer = XmlWriter.Create(stream, settings);
        var namespaces = new XmlSerializerNamespaces();
        namespaces.Add("", "");
        var serializer = new XmlSerializer(value.GetType());
        serializer.Serialize(writer, value, namespaces);
        return Encoding.UTF8.GetString(stream.ToArray());
    }

    /// <summary>
    /// 将XML字符串反序列化成指定泛型的对象（使用.NET内置XML序列化）。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="xml">XML字符串。</param>
    /// <returns>泛型对象。</returns>
    public static T FromXml<T>(string xml) where T : class
    {
        if (string.IsNullOrWhiteSpace(xml))
            return default;

        using var stream = new MemoryStream(Encoding.UTF8.GetBytes(xml));
        using var reader = new StreamReader(stream, Encoding.UTF8);
        var serializer = new XmlSerializer(typeof(T));
        return (T)serializer.Deserialize(reader);
    }

    /// <summary>
    /// 判断字符串是否为JSON格式。
    /// </summary>
    /// <param name="input">字符串。</param>
    /// <returns></returns>
    public static bool IsJson(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return false;

        if (!(input.StartsWith('{') && input.EndsWith('}')) &&
            !(input.StartsWith('[') && input.EndsWith(']')))
            return false;

        return true;
    }

    /// <summary>
    /// 将对象序列化为JSON字符串（使用.NET内置JSON序列化）。
    /// </summary>
    /// <param name="value">对象。</param>
    /// <returns>JSON字符串。</returns>
    public static string ToJson(object value)
    {
        if (value == null)
            return string.Empty;

        return JsonSerializer.Serialize(value, new JsonSerializerOptions
        {
            Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }

    /// <summary>
    /// 将JSON字符串转成动态对象（使用.NET内置JSON序列化）。
    /// </summary>
    /// <param name="json">JSON字符串。</param>
    /// <returns>动态对象。</returns>
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
        UnmappedMemberHandling = JsonUnmappedMemberHandling.Skip,
        NumberHandling = JsonNumberHandling.AllowReadingFromString
    };

    private static string FormatDSJson(string json) => json.Replace("{}", "null").Replace("\"\"", "null");

    /// <summary>
    /// 将JSON字符串反序列化成指定泛型的对象（使用.NET内置JSON序列化）。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="json">JSON字符串。</param>
    /// <returns>泛型对象。</returns>
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
            HandleJsonException(typeof(T), json, ex);
            return default;
        }
    }

    /// <summary>
    /// 将JSON字符串反序列化成指定类型的对象（使用.NET内置JSON序列化）。
    /// </summary>
    /// <param name="type">类型。</param>
    /// <param name="json">JSON字符串。</param>
    /// <returns>对象。</returns>
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
            HandleJsonException(type, json, ex);
            return null;
        }
    }

    private static void HandleJsonException(Type type, string json, Exception ex)
    {
        var message = $@"
========================================================
TYPE：{type.FullName}
JSON：{json}
{ex}
--------------------------------------------------------";
        Logger.Error(LogTarget.JSON, new UserInfo { Name = "JSON" }, message);
    }

    /// <summary>
    /// 将一个对象映射成指定泛型的对象，两个类型的属性应是包含或包含于关系。
    /// 通过序列化JSON，再反序列JSON实现。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="value">原对象。</param>
    /// <returns>泛型对象。</returns>
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
    /// <summary>
    /// 根据资源文件名获取程序集中资源文件内容。
    /// </summary>
    /// <param name="assembly">程序集。</param>
    /// <param name="name">资源文件名。</param>
    /// <returns>资源文件内容。</returns>
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

    /// <summary>
    /// 确信文件路径可用，如果没创建，则创建。
    /// </summary>
    /// <param name="fileName">文件路径。</param>
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

    /// <summary>
    /// 复制文件。
    /// </summary>
    /// <param name="sourceFileName">原文件路径。</param>
    /// <param name="destFileName">目标文件路径。</param>
    /// <param name="overwrite">是否覆盖目标文件，默认True。</param>
    public static void CopyFile(string sourceFileName, string destFileName, bool overwrite = true)
    {
        var info = new FileInfo(destFileName);
        if (!info.Directory.Exists)
            info.Directory.Create();

        File.Copy(sourceFileName, destFileName, overwrite);
    }

    /// <summary>
    /// 读取一个文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <returns></returns>
    public static string ReadFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return string.Empty;

        if (!File.Exists(path))
            return string.Empty;

        return File.ReadAllText(path);
    }

    /// <summary>
    /// 异步读取一个文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <returns></returns>
    public static Task<string> ReadFileAsync(string path)
    {
        if (string.IsNullOrEmpty(path))
            return Task.FromResult(string.Empty);

        if (!File.Exists(path))
            return Task.FromResult(string.Empty);

        return File.ReadAllTextAsync(path);
    }

    /// <summary>
    /// 保存文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="content">文件内容。</param>
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

    /// <summary>
    /// 异步保存文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="content">文件内容。</param>
    public static Task SaveFileAsync(string path, string content)
    {
        if (string.IsNullOrEmpty(path))
            return Task.CompletedTask;

        if (string.IsNullOrEmpty(content))
            return Task.CompletedTask;

        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        return File.WriteAllTextAsync(path, content);
    }

    /// <summary>
    /// 保存文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="bytes">文件字节数组。</param>
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

    /// <summary>
    /// 异步保存文件内容。
    /// </summary>
    /// <param name="path">文件路径。</param>
    /// <param name="bytes">文件字节数组。</param>
    public static Task SaveFileAsync(string path, byte[] bytes)
    {
        if (string.IsNullOrEmpty(path))
            return Task.CompletedTask;

        if (bytes == null || bytes.Length == 0)
            return Task.CompletedTask;

        var info = new FileInfo(path);
        if (!info.Directory.Exists)
            info.Directory.Create();

        return File.WriteAllBytesAsync(path, bytes);
    }

    /// <summary>
    /// 删除一个文件。
    /// </summary>
    /// <param name="path">文件路径。</param>
    public static void DeleteFile(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;

        if (!File.Exists(path))
            return;

        File.Delete(path);
    }

    /// <summary>
    /// 重命名文件名。
    /// </summary>
    /// <param name="sourceFile">原文件。</param>
    /// <param name="newName">新文件名。</param>
    /// <exception cref="FileNotFoundException"></exception>
    public static void RenameFile(string sourceFile, string newName)
    {
        var directory = Path.GetDirectoryName(sourceFile);
        if (directory == null)
            throw new FileNotFoundException($"The file '{sourceFile}' does not have a parent directory.");

        var extName = Path.GetExtension(sourceFile);
        var destFile = Path.Combine(directory, newName + extName);
        File.Move(sourceFile, destFile);
    }

    /// <summary>
    /// 重命名文件夹名称。
    /// </summary>
    /// <param name="sourceDir">原文件夹。</param>
    /// <param name="newName">新文件夹名。</param>
    /// <exception cref="DirectoryNotFoundException"></exception>
    public static void RenameDirectory(string sourceDir, string newName)
    {
        var parentDir = Path.GetDirectoryName(sourceDir);
        if (parentDir == null)
            throw new DirectoryNotFoundException($"The directory '{sourceDir}' does not have a parent directory.");

        var destDir = Path.Combine(parentDir, newName);
        Directory.Move(sourceDir, destDir);
    }

    /// <summary>
    /// 删除文件夹及子文件夹和文件。
    /// </summary>
    /// <param name="targetDir">文件夹目录。</param>
    public static void DeleteDirectory(string targetDir)
    {
        if (string.IsNullOrEmpty(targetDir))
            return;

        if (!Directory.Exists(targetDir))
            return;

        File.SetAttributes(targetDir, FileAttributes.Normal);

        string[] files = Directory.GetFiles(targetDir);
        string[] dirs = Directory.GetDirectories(targetDir);

        foreach (string file in files)
        {
            File.SetAttributes(file, FileAttributes.Normal);
            File.Delete(file);
        }

        foreach (string dir in dirs)
        {
            DeleteDirectory(dir);
        }

        Directory.Delete(targetDir, false);
    }

    /// <summary>
    /// 复制文件夹到新位置。
    /// </summary>
    /// <param name="sourceDir">原文件夹。</param>
    /// <param name="destDir">新文件夹。</param>
    public static void CopyDirectory(string sourceDir, string destDir)
    {
        // 创建目标目录
        Directory.CreateDirectory(destDir);

        // 复制所有文件
        foreach (var file in Directory.GetFiles(sourceDir))
        {
            var name = Path.GetFileName(file);
            var destFile = Path.Combine(destDir, name);
            File.Copy(file, destFile, true);
        }

        // 递归复制所有子目录
        foreach (var dir in Directory.GetDirectories(sourceDir))
        {
            var name = Path.GetFileName(dir);
            var destSubDir = Path.Combine(destDir, name);
            CopyDirectory(dir, destSubDir);
        }
    }
    #endregion

    #region Color
    /// <summary>
    /// 获取一个随机颜色。
    /// </summary>
    /// <returns>Color对象。</returns>
    public static Color GetRandomColor()
    {
        var random = new Random();
        return Color.FromArgb(random.Next(255), random.Next(255), random.Next(255));
    }

    /// <summary>
    /// 根据HTML颜色转换成Color对象。
    /// </summary>
    /// <param name="htmlColor">HTML颜色字符串，如：#FFFFFF。</param>
    /// <returns>Color对象。</returns>
    public static Color FromHtml(string htmlColor) => ColorTranslator.FromHtml(htmlColor);

    /// <summary>
    /// 将Color对象转换成HTML颜色字符串。
    /// </summary>
    /// <param name="color">Color对象。</param>
    /// <returns>HTML颜色字符串，如：#FFFFFF。</returns>
    public static string ToHtml(Color color) => ColorTranslator.ToHtml(color);
    #endregion

    #region Byte
    /// <summary>
    /// 将流对象转换成字节数组。
    /// </summary>
    /// <param name="stream">流对象。</param>
    /// <returns>字节数组。</returns>
    public static byte[] StreamToBytes(Stream stream)
    {
        if (stream == null)
            return null;

        var bytes = new byte[stream.Length];
        stream.ReadExactly(bytes);
        stream.Seek(0, SeekOrigin.Begin);
        return bytes;
    }

    /// <summary>
    /// 将HEX编码转换成字节数组。
    /// </summary>
    /// <param name="hexString">HEX编码字符串。</param>
    /// <returns>字节数组。</returns>
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

    /// <summary>
    /// 将字节数组转换成HEX编码字符串。
    /// </summary>
    /// <param name="bytes">字节数组。</param>
    /// <param name="separator">HEX编码分隔符，默认空格。</param>
    /// <returns>HEX编码字符串。</returns>
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
}