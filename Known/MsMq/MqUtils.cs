using System.IO;
using System.Messaging;
using System.Text;

namespace Known.MsMq
{
    public class MqUtils
    {
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
