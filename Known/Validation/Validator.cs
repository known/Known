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
    }
}
