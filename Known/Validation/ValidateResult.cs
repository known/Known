using System;
using System.Collections.Generic;
using System.Linq;

namespace Known.Validation
{
    /// <summary>
    /// 验证结果类。
    /// </summary>
    public class ValidateResult
    {
        private List<ValidInfo> validInfos;

        /// <summary>
        /// 构造函数，创建一个验证结果类实例。
        /// </summary>
        /// <param name="validInfos">验证信息集合。</param>
        public ValidateResult(List<ValidInfo> validInfos)
        {
            this.validInfos = validInfos ?? new List<ValidInfo>();
        }

        /// <summary>
        /// 取得是否有错误消息。
        /// </summary>
        public bool HasError
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Error); }
        }

        /// <summary>
        /// 取得是否有警告信息。
        /// </summary>
        public bool HasWarn
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Warn); }
        }

        /// <summary>
        /// 取得是否有信息消息。
        /// </summary>
        public bool HasInfo
        {
            get { return validInfos.Exists(v => v.Level == ValidLevel.Info); }
        }

        /// <summary>
        /// 取得错误消息。
        /// </summary>
        public string ErrorMessage
        {
            get { return GetMessages(ValidLevel.Error); }
        }

        /// <summary>
        /// 取得警告消息。
        /// </summary>
        public string WarnMessage
        {
            get { return GetMessages(ValidLevel.Warn); }
        }

        /// <summary>
        /// 取得信息消息。
        /// </summary>
        public string InfoMessage
        {
            get { return GetMessages(ValidLevel.Info); }
        }

        private string GetMessages(ValidLevel level)
        {
            if (validInfos == null || validInfos.Count == 0)
                return string.Empty;

            var messages = validInfos.Where(v => v.Level == level);
            return string.Join(Environment.NewLine, messages);
        }
    }
}