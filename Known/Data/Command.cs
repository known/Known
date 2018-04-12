using System.Collections.Generic;
using System.Text;
using Known.Extensions;

namespace Known.Data
{
    /// <summary>
    /// 数据库命令类。
    /// </summary>
    public class Command
    {
        /// <summary>
        /// 构造函数，创建一个数据库命令类实例。
        /// </summary>
        /// <param name="text">SQL语句。</param>
        public Command(string text)
        {
            Text = text;
            Parameters = new Dictionary<string, object>();
        }

        /// <summary>
        /// 构造函数，创建一个数据库命令类实例。
        /// </summary>
        /// <param name="text">SQL语句。</param>
        /// <param name="parameters">SQL语句参数字典。</param>
        public Command(string text, Dictionary<string, object> parameters)
        {
            Text = text;
            Parameters = parameters;
        }

        /// <summary>
        /// 取得SQL语句。
        /// </summary>
        public string Text { get; }

        /// <summary>
        /// 取得SQL语句参数字典。
        /// </summary>
        public Dictionary<string, object> Parameters { get; }

        /// <summary>
        /// 取得命令是否含有SQL参数。
        /// </summary>
        public bool HasParameter
        {
            get { return Parameters.Count > 0; }
        }

        /// <summary>
        /// 添加SQL语句参数。
        /// </summary>
        /// <param name="name">参数名。</param>
        /// <param name="value">参数值。</param>
        public void AddParameter(string name, object value)
        {
            Parameters[name] = value;
        }

        /// <summary>
        /// 获取数据库命令的打印字符串，包含命令SQL语句和参数信息。
        /// </summary>
        /// <returns>命令SQL语句和参数。</returns>
        public override string ToString()
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Text={Text}");
            if (HasParameter)
            {
                var parameters = Parameters.ToJson();
                sb.AppendLine($"Parameters:{parameters}");
            }
            return sb.ToString();
        }
    }
}
