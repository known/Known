using System;
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
        /// 
        /// </summary>
        /// <param name="broken"></param>
        /// <param name="message"></param>
        public void AddError(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Error, message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broken"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void AddError(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddError(broken, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broken"></param>
        /// <param name="message"></param>
        public void AddWarn(bool broken, string message)
        {
            if (broken)
            {
                validInfos.Add(new ValidInfo(ValidLevel.Warn, message));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="broken"></param>
        /// <param name="format"></param>
        /// <param name="args"></param>
        public void AddWarn(bool broken, string format, params object[] args)
        {
            var message = string.Format(format, args);
            AddError(broken, message);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Result ToResult()
        {
            if (validInfos != null && validInfos.Count > 0)
            {
                return Result.Error(string.Join(Environment.NewLine, validInfos));
            }

            return Result.Success("");
        }
    }
}
