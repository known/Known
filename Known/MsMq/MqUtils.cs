using System.IO;
using System.Messaging;
using System.Text;

namespace Known.MsMq
{
    /// <summary>
    /// MQ效用类。
    /// </summary>
    public class MqUtils
    {
        /// <summary>
        /// 发送MQ消息。
        /// </summary>
        /// <param name="path">MQ地址。</param>
        /// <param name="label">标签。</param>
        /// <param name="content">内容。</param>
        public static void SendMessage(string path, string label, string content)
        {
            using (var queue = new MessageQueue(path))
            {
                var message = new Message
                {
                    Label = label,
                    Formatter = new BinaryMessageFormatter(),
                    BodyStream = new MemoryStream(Encoding.UTF8.GetBytes(content))
                };
                queue.Send(message, MessageQueueTransactionType.Single);
            }
        }
    }
}
