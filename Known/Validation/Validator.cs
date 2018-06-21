using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text.RegularExpressions;
using Known.Extensions;

namespace Known.Validation
{
    /// <summary>
    /// 验证器类，提供逻辑验证操作。
    /// </summary>
    public class Validator
    {
        private List<ValidInfo> validInfos = new List<ValidInfo>();

        /// <summary>
        /// 构造函数，创建一个验证器实例。
        /// </summary>
        public Validator() { }

        /// <summary>
        /// 构造函数，创建一个验证器实例。
        /// </summary>
        /// <param name="errors">错误消息集合。</param>
        public Validator(List<string> errors)
        {
            validInfos.AddRange(errors.Select(e => new ValidInfo(ValidLevel.Error, e)));
        }

        /// <summary>
        /// 构造函数，创建一个验证器实例。
        /// </summary>
        /// <param name="infos">验证信息集合。</param>
        public Validator(List<ValidInfo> infos)
        {
            validInfos.AddRange(infos);
        }

        /// <summary>
        /// 添加错误。
        /// </summary>
        /// <param name="broken">错误判断。</param>
        /// <param name="message">错误消息。</param>
        public void AddError(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Error, message));
            }
        }

        /// <summary>
        /// 添加错误。
        /// </summary>
        /// <param name="broken">错误判断。</param>
        /// <param name="format">错误消息格式。</param>
        /// <param name="args">错误消息格式参数。</param>
        public void AddError(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddError(broken, message);
        }

        /// <summary>
        /// 添加警告。
        /// </summary>
        /// <param name="broken">警告判断。</param>
        /// <param name="message">警告消息。</param>
        public void AddWarn(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Warn, message));
            }
        }

        /// <summary>
        /// 添加警告。
        /// </summary>
        /// <param name="broken">警告判断。</param>
        /// <param name="format">警告消息格式。</param>
        /// <param name="args">警告消息格式参数。</param>
        public void AddWarn(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddError(broken, message);
        }

        /// <summary>
        /// 获取验证结果信息。
        /// </summary>
        /// <returns>验证结果信息。</returns>
        public ValidateResult ToResult()
        {
            return new ValidateResult(validInfos);
        }

        /// <summary>
        /// 验证数据表栏位是否为非空字符串。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <returns>数据表栏位数据。</returns>
        public static string ValidateNotEmptyString(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<string>(fieldName);
            if (string.IsNullOrWhiteSpace(value))
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        /// <summary>
        /// 验证数据表栏位是否为非空整数。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <returns>数据表栏位数据。</returns>
        public static int? ValidateNotEmptyInt(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<int?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        /// <summary>
        /// 验证数据表栏位是否为整数。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <returns>数据表栏位数据。</returns>
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

        /// <summary>
        /// 验证数据表栏位是否为非空浮点数。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <returns>数据表栏位数据。</returns>
        public static decimal? ValidateNotEmptyDecimal(List<string> messages, DataRow row, string fieldName)
        {
            var value = row.Get<decimal?>(fieldName);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空！");
            return value;
        }

        /// <summary>
        /// 验证数据表栏位是否为浮点数。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <returns>数据表栏位数据。</returns>
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

        /// <summary>
        /// 验证数据表栏位是否为指定格式的非空日期。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <param name="format">日期格式字符串，例：yyyy-MM-dd。</param>
        /// <returns>数据表栏位数据。</returns>
        public static DateTime? ValidateNotEmptyDateTime(List<string> messages, DataRow row, string fieldName, string format)
        {
            var value = row.Get<string>(fieldName).ToDateTime(format);
            if (!value.HasValue)
                messages.Add(fieldName + "不能为空且格式必须是" + format + "！");
            return value;
        }

        /// <summary>
        /// 验证数据表栏位是否为指定格式的日期。
        /// </summary>
        /// <param name="messages">错误信息集合。</param>
        /// <param name="row">数据行。</param>
        /// <param name="fieldName">栏位名。</param>
        /// <param name="format">日期格式字符串，例：yyyy-MM-dd。</param>
        /// <returns>数据表栏位数据。</returns>
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

        /// <summary>
        /// 验证字符串是否是数值。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsNumber(string input)
        {
            return Regex.IsMatch(input, @"^[0-9]*$");
        }

        /// <summary>
        /// 验证字符串是否是邮箱。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsEmail(string input)
        {
            return Regex.IsMatch(input, @"^([a-zA-Z0-9_-])+@([a-zA-Z0-9_-])+((\.[a-zA-Z0-9_-]{2,3}){1,2})$");
        }

        /// <summary>
        /// 验证字符串是否是网址。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsUrl(string input)
        {
            return Regex.IsMatch(input, @"http(s)?://([/w-]+/.)+[/w-]+(/[/w- ./?%&=]*)?");
        }

        /// <summary>
        /// 验证字符串是否是固定电话号码。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsPhone(string input)
        {
            return Regex.IsMatch(input, @"^(\d{3,4}-)?\d{6,8}$");
        }

        /// <summary>
        /// 验证字符串是否是手机号码。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsMobile(string input)
        {
            return Regex.IsMatch(input, @"^[1]+[3,5]+\d{9}");
        }

        /// <summary>
        /// 验证字符串是否是身份证号码。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsIDCard(string input)
        {
            return Regex.IsMatch(input, @"(^\d{18}$)|(^\d{15}$)");
        }

        /// <summary>
        /// 验证字符串是否是邮政编码。
        /// </summary>
        /// <param name="input">输入的字符串。</param>
        /// <returns>是否验证通过。</returns>
        public static bool IsPostalcode(string input)
        {
            return Regex.IsMatch(input, @"^\d{6}$");
        }
    }
}
