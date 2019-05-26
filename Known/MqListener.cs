using System;
using System.Diagnostics;
using System.IO;
using System.Messaging;
using System.Text;

namespace Known
{
    /// <summary>
    /// MSMQ 消息监听类。
    /// </summary>
    public class MqListener
    {
        private bool listen;
        private readonly MessageQueue queue;
        private Action<MqMessage> action;

        /// <summary>
        /// 创建一个 MSMQ 消息监听类实例。
        /// </summary>
        /// <param name="config">MQ 配置信息。</param>
        public MqListener(MqConfigInfo config)
        {
            Config = config ?? throw new ArgumentNullException(nameof(config));

            queue = new MessageQueue
            {
                QueueName = config.Name,
                Path = config.Path,
                Formatter = new BinaryMessageFormatter()
            };
        }

        /// <summary>
        /// 取得 MQ 配置信息。
        /// </summary>
        public MqConfigInfo Config { get; }

        /// <summary>
        /// 启动 MSMQ 消息监听。
        /// </summary>
        /// <param name="action">接收消息时的处理操作。</param>
        public void Start(Action<MqMessage> action)
        {
            this.action = action;
            listen = true;
            queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
            StartListening();
        }

        /// <summary>
        /// 停止 MSMQ 消息监听。
        /// </summary>
        public void Stop()
        {
            listen = false;
            queue.PeekCompleted -= new PeekCompletedEventHandler(OnPeekCompleted);
            queue.ReceiveCompleted -= new ReceiveCompletedEventHandler(OnReceiveCompleted);
        }

        private void StartListening()
        {
            if (!listen)
                return;

            if (queue.Transactional)
                queue.BeginPeek();
            else
                queue.BeginReceive();
        }

        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            queue.EndPeek(e.AsyncResult);
            var trans = new MessageQueueTransaction();
            try
            {
                trans.Begin();
                Message message = queue.Receive(trans);
                trans.Commit();

                StartListening();
                FireReceiveEvent(Config.Name, message);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex);
                trans.Abort();
            }
        }

        private void OnReceiveCompleted(object sender, ReceiveCompletedEventArgs e)
        {
            var message = queue.EndReceive(e.AsyncResult);
            StartListening();
            FireReceiveEvent(Config.Name, message);
        }

        private void FireReceiveEvent(string queueName, Message message)
        {
            if (action == null || message == null)
                return;

            action(new MqMessage
            {
                QueueName = queueName,
                Label = message.Label,
                BodyStream = message.BodyStream,
                TimeToBeReceived = message.TimeToBeReceived,
                TimeToReachQueue = message.TimeToReachQueue
            });
        }
    }

    /// <summary>
    /// MQ 配置信息类。
    /// </summary>
    public class MqConfigInfo
    {
        /// <summary>
        /// 取得或设置 MQ 名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置 MQ 地址。
        /// </summary>
        public string Path { get; set; }
    }

    /// <summary>
    /// MQ 接收到的消息内容类。
    /// </summary>
    public class MqMessage
    {
        /// <summary>
        /// 取得或设置 MSMQ 队列名称。
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 取得或设置 MQ 消息的标签。
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 取得或设置 MQ 的消息内容流。
        /// </summary>
        public Stream BodyStream { get; set; }

        /// <summary>
        /// 取得或设置队列接收消息的最长时间。
        /// </summary>
        public TimeSpan TimeToBeReceived { get; set; }

        /// <summary>
        /// 取得或设置到达队列的最长时间。
        /// </summary>
        public TimeSpan TimeToReachQueue { get; set; }
    }

    /// <summary>
    /// MSMQ 效用类。
    /// </summary>
    public sealed class MqUtils
    {
        /// <summary>
        /// 发送 MQ 消息。
        /// </summary>
        /// <param name="path">MQ 地址。</param>
        /// <param name="label">消息标签。</param>
        /// <param name="content">消息内容，默认 UTF8 格式。</param>
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
