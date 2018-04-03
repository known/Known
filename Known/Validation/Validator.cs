using System.Collections.Generic;
using System.Linq;

namespace Known.Validation
{
    /// <summary>
    /// 验证器类，提供逻辑验证操作。
    /// </summary>
    public class Validator
    {
        private List<ValidInfo> validInfos = new List<ValidInfo>();

        /// <summary>
        /// 构造函数，创建验证器实例。
        /// </summary>
        public Validator() { }

        /// <summary>
        /// 构造函数，创建验证器实例。
        /// </summary>
        /// <param name="errors">错误消息集合。</param>
        public Validator(List<string> errors)
        {
            validInfos.AddRange(errors.Select(e => new ValidInfo(ValidLevel.Error, e)));
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
    }
}
