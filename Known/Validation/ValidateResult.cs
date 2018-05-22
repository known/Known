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
        /// <summary>
        /// 构造函数，创建一个验证结果类实例。
        /// </summary>
        /// <param name="infos">验证信息集合。</param>
        internal ValidateResult(List<ValidInfo> infos)
        {
            Infos = infos ?? new List<ValidInfo>();
        }

        /// <summary>
        /// 取得验证信息集合。
        /// </summary>
        public List<ValidInfo> Infos { get; }

        /// <summary>
        /// 取得是否有错误消息。
        /// </summary>
        public bool HasError
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Error); }
        }

        /// <summary>
        /// 取得是否有警告信息。
        /// </summary>
        public bool HasWarn
        {
            get { return Infos.Exists(v => v.Level == ValidLevel.Warn); }
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

        private string GetMessages(ValidLevel level)
        {
            if (Infos == null || Infos.Count == 0)
                return string.Empty;

            var messages = Infos.Where(v => v.Level == level);
            return string.Join(Environment.NewLine, messages);
        }
    }
}