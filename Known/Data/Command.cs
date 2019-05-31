using System;
using System.Collections.Generic;
using System.Text;
using Known.Extensions;

namespace Known.Data
{
    /// <summary>
    /// 数据访问命令类。
    /// </summary>
    public class Command
    {
        internal Command(string text, Dictionary<string, object> parameters = null)
        {
            if (string.IsNullOrWhiteSpace(text))
                throw new ArgumentNullException(nameof(text));

            Text = text;
            Parameters = parameters ?? new Dictionary<string, object>();
        }

        /// <summary>
        /// 取得数据访问 SQL 语句。
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 取得数据访问 SQL 语句的参数字典。
        /// </summary>
        public Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// 取得命令是否有参数字典。
        /// </summary>
        public bool HasParameter
        {
            get { return Parameters.Count > 0; }
        }

        /// <summary>
        /// 添加数据访问命令参数。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <param name="value">参数值。</param>
        public void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }

        /// <summary>
        /// 重写 ToString，显示数据访问命令的 SQL 语句和参数字典。
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Text={Text}");
            if (HasParameter)
            {
                var parameters = Parameters.ToJson();
                sb.AppendLine($"Parameters={parameters}");
            }
            return sb.ToString();
        }
    }
}
