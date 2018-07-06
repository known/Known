using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known.Validation
{
    public class Validator
    {
        private List<ValidInfo> validInfos = new List<ValidInfo>();

        public Validator() { }

        public Validator(List<string> errors)
        {
            validInfos.AddRange(errors.Select(e => new ValidInfo(ValidLevel.Error, e)));
        }

        public Validator(List<ValidInfo> infos)
        {
            validInfos.AddRange(infos);
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
            AddError(broken, message);
        }

        public ValidateResult ToResult()
        {
            return new ValidateResult(validInfos);
        }

        public static string ValidateNotEmptyString(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(value))
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        public static int? ValidateNotEmptyInt(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<int?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        public static int? ValidateInt(List<string> messages, DataRow row, string fieldName)
        {
            var text = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var value = row.Get<int?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "格式不正确！");
            return value;
        }

        public static decimal? ValidateNotEmptyDecimal(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<decimal?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        public static decimal? ValidateDecimal(List<string> messages, DataRow row, string fieldName)
        {
            var text = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var value = row.Get<decimal?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "格式不正确！");
            return value;
        }

        public static DateTime? ValidateNotEmptyDateTime(List<string> messages, DataRow row, string fieldName, string format)
        {
            var value = row.Get<string>(fieldName).ToDateTime(format);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空且格式必须是" + format + "！");
            return value;
        }

        public static DateTime? ValidateDateTime(List<string> messages, DataRow row, string fieldName, string format)
        {
            var text = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(text))
                return null;

            var value = text.ToDateTime(format);
            if (!value.HasValue)
                messages.Add(fieldName + "格式必须是" + format + "！");
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
            //var pattern = @"^[a-zA-Z0-9_+.-]+\@([a-zA-Z0-9-]+\.)+[a-zA-Z0-9]{2,4}$");
            return Regex.IsMatch(input, pattern);
        }

        public static bool IsUrl(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
                return false;

            return Regex.IsMatch(input, @"http(s)?://([/w-]+/.)+[/w-]+(/[/w- ./?%&=]*)?");
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
}
