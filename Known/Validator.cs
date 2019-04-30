using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known
{
    public class Validator
    {
        private readonly List<ValidInfo> validInfos = new List<ValidInfo>();

        internal Validator(List<ValidInfo> infos)
        {
            validInfos.AddRange(infos);
        }

        public bool HasError
        {
            get { return new ValidateResult(validInfos).HasError; }
        }

        public bool HasWarn
        {
            get { return new ValidateResult(validInfos).HasWarn; }
        }

        public string ErrorMessage
        {
            get { return new ValidateResult(validInfos).ErrorMessage; }
        }

        public string WarnMessage
        {
            get { return new ValidateResult(validInfos).WarnMessage; }
        }

        public void AddError(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Error, message));
            }
        }

        public void AddError(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddError(broken, message);
        }

        public void AddWarn(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Warn, message));
            }
        }

        public void AddWarn(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddWarn(broken, message);
        }

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

        public static bool IsNumber(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[0-9]*$");
        }

        public static bool IsEmail(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            var pattern = @"\w+([-+.]\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*";
            //var pattern = "^(?:[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+\\.)*[\\w\\!\\#\\$\\%\\&\\'\\*\\+\\-\\/\\=\\?\\^\\`\\{\\|\\}\\~]+@(?:(?:(?:[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!\\.)){0,61}[a-zA-Z0-9]?\\.)+[a-zA-Z0-9](?:[a-zA-Z0-9\\-](?!$)){0,61}[a-zA-Z0-9]?)|(?:\\[(?:(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\.){3}(?:[01]?\\d{1,2}|2[0-4]\\d|25[0-5])\\]))$";
            //var pattern = @"^[a-zA-Z0-9_+.-]+\@([a-zA-Z0-9-]+\.)+[a-zA-Z0-9]{2,4}$");
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"http(s)?://([/w-]+/.)+[/w-]+(/[/w- ./?%&=]*)?");
        }

        public static bool IsIpAddress(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return IPAddress.TryParse(input, out IPAddress ip);
        }

        public static bool IsPhone(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^(\d{3,4}-)?\d{6,8}$");
        }

        public static bool IsMobile(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"^[1]+[3,5]+\d{9}");
        }

        public static bool IsIDCard(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"(^\d{18}$)|(^\d{15}$)");
        }

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
