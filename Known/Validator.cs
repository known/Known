using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known
{
    /// <summary>
    /// 验证者类。
    /// </summary>
    public class Validator
    {
        private readonly List<ValidInfo> validInfos = new List<ValidInfo>();

        internal Validator(List<ValidInfo> infos)
        {
            validInfos.AddRange(infos);
        }

        /// <summary>
        /// 取得是否有错误信息。
        /// </summary>
        public bool HasError
        {
            get { return new ValidateResult(validInfos).HasError; }
        }

        /// <summary>
        /// 取得是否有警告信息。
        /// </summary>
        public bool HasWarn
        {
            get { return new ValidateResult(validInfos).HasWarn; }
        }

        /// <summary>
        /// 取得错误信息。
        /// </summary>
        public string ErrorMessage
        {
            get { return new ValidateResult(validInfos).ErrorMessage; }
        }

        /// <summary>
        /// 取得警告信息。
        /// </summary>
        public string WarnMessage
        {
            get { return new ValidateResult(validInfos).WarnMessage; }
        }

        /// <summary>
        /// 添加错误信息。
        /// </summary>
        /// <param name="broken">是否为错误。</param>
        /// <param name="message">错误信息。</param>
        public void AddError(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Error, message));
            }
        }

        /// <summary>
        /// 添加警告信息。
        /// </summary>
        /// <param name="broken">是否为警告。</param>
        /// <param name="message">警告信息。</param>
        public void AddWarn(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Warn, message));
            }
        }

        /// <summary>
        /// 验证指定类型的非空导入栏位。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="messages">错误信息列表。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位字段名。</param>
        /// <param name="format">日期类型格式，例：yyyy-MM-dd。</param>
        /// <returns>导入栏位值。</returns>
        public static T ValidateNotEmpty<T>(List<string> messages, DataRow row, string fieldName, string format = null)
        {
            if (!string.IsNullOrWhiteSpace(format))
            {
                var time = row.Get<string>(fieldName).ToDateTime(format);
                if (!time.HasValue)
                    messages.Add($"{fieldName}不能为空且格式必须是{format}！");

                return Utils.ConvertTo<T>(time);
            }

            var value = row.Get<T>(fieldName);
            if (value == null)
                messages.Add($"{fieldName}不能为空！");

            return value;
        }

        /// <summary>
        /// 验证指定类型的可空导入栏位。
        /// </summary>
        /// <typeparam name="T">数据类型。</typeparam>
        /// <param name="messages">错误信息列表。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位字段名。</param>
        /// <param name="format">日期类型格式，例：yyyy-MM-dd。</param>
        /// <returns>导入栏位值。</returns>
        public static T Validate<T>(List<string> messages, DataRow row, string fieldName, string format = null)
        {
            var text = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(text))
                return default;

            if (!string.IsNullOrWhiteSpace(format))
            {
                var time = text.ToDateTime(format);
                if (!time.HasValue)
                    messages.Add($"{fieldName}格式必须是{format}！");

                return Utils.ConvertTo<T>(time);
            }

            var value = row.Get<T>(fieldName);
            if (value == null)
                messages.Add($"{fieldName}格式不正确！");

            return value;
        }

        /// <summary>
        /// 验证指定类型的枚举导入栏位。
        /// </summary>
        /// <typeparam name="T">枚举类型。</typeparam>
        /// <param name="messages">错误信息列表。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位字段名。</param>
        /// <returns>导入栏位值。</returns>
        public static T ValidateEnum<T>(List<string> messages, DataRow row, string fieldName) where T : struct
        {
            var text = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(text))
            {
                messages.Add($"{fieldName}不能为空！");
                return default;
            }

            if (!Enum.TryParse(text, out T value))
            {
                var values = typeof(T).ToDictionary().Select(c => $"{c.Key}-{c.Value}");
                var valueString = string.Join("、", values);
                messages.Add($"{fieldName}必须填写枚举值（{valueString}）！");
            }

            return value;
        }

        /// <summary>
        /// 判断字符串是否是数字。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[0-9]*$");
        }

        /// <summary>
        /// 判断字符串是否是邮件地址。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            //var pattern = "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$";
            //var pattern = @"^[a-zA-Z0-9_+.-]+\@([a-zA-Z0-9-]+\.)+[a-zA-Z0-9]{2,4}$");
            return Regex.IsMatch(input, pattern);
        }

        /// <summary>
        /// 判断字符串是否是网址。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"http(s)?://([/w-]+/.)+[/w-]+(/[/w- ./?%&=]*)?");
        }

        /// <summary>
        /// 判断字符串是否是 IP 地址。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsIpAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return IPAddress.TryParse(input, out _);
        }

        /// <summary>
        /// 判断字符串是否是电话号码。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsPhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// 判断字符串是否是手机号码。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsMobile(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[1]+[3,5]+\d{9}");
        }

        /// <summary>
        /// 判断字符串是否是身份证号码。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsIDCard(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"(^\d{18}$)|(^\d{15}$)");
        }

        /// <summary>
        /// 判断字符串是否是邮政编码。
        /// </summary>
        /// <param name="input">字符串。</param>
        /// <returns>返回 True 或 False。</returns>
        public static bool IsPostalcode(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^\d{6}$");
        }
    }

    enum ValidLevel
    {
        Error,
        Warn
    }

    class ValidInfo
    {
        internal ValidInfo(ValidLevel level, string message)
        {
            Level = level;
            Message = message;
        }

        internal ValidInfo(ValidLevel level, string field, List<string> messages)
        {
            Level = level;
            Field = field;
            Message = string.Join(Environment.NewLine, messages);
        }

        public ValidLevel Level { get; }
        public string Field { get; }
        public string Message { get; }
    }

    class ValidateResult
    {
        internal ValidateResult(List<ValidInfo> infos)
        {
            Infos = infos ?? new List<ValidInfo>();
        }

        public List<ValidInfo> Infos { get; }

        public bool HasError
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Error); }
        }

        public bool HasWarn
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Warn); }
        }

        public string ErrorMessage
        {
            get { return GetMessages(ValidLevel.Error); }
        }

        public string WarnMessage
        {
            get { return GetMessages(ValidLevel.Warn); }
        }

        private string GetMessages(ValidLevel level)
        {
            if (Infos == null || Infos.Count == 0)
                return string.Empty;

            var messages = Infos.Where(v => v.Level == level).Select(v => v.Message);
            return string.Join(Environment.NewLine, messages);
        }
    }
}
