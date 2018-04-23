using System;
using System.Diagnostics;
using System.Messaging;

namespace Known.MsMq
{
    /// <summary>
    /// MQ监听者。
    /// </summary>
    public class MqListener
    {
        private bool listen;
        private MessageQueue queue;
        private Action<string, Message> action;

        /// <summary>
        /// 构造函数，创建一个MQ监听者实例。
        /// </summary>
        /// <param name="config">MQ配置信息。</param>
        public MqListener(MqConfigInfo config)
        {
            Config = config ?? throw new ArgumentNullException("config");

            queue = new MessageQueue
            {
                QueueName = config.Name,
                Path = config.Path,
                Formatter = new BinaryMessageFormatter()
            };
        }

        /// <summary>
        /// 取得MQ配置信息。
        /// </summary>
        public MqConfigInfo Config { get; }

        /// <summary>
        /// 开始监听。
        /// </summary>
        /// <param name="action">监听消息处理者。</param>
        public void Start(Action<string, Message> action)
        {
            this.action = action;
            listen = true;
            queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
            StartListening();
        }

        /// <summary>
        /// 停止监听。
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

            // 异步接收BeginReceive()方法无MessageQueueTransaction重载(微软类库的Bug?)
            // 这里变通一下：先异步BeginPeek()，然后带事务异步接收Receive(MessageQueueTransaction)
            if (queue.Transactional)
                queue.BeginPeek();
            else
                queue.BeginReceive();
        }

        private void OnPeekCompleted(object sender, PeekCompletedEventArgs e)
        {
            queue.EndPeek(e.AsyncResult);
            var trans = new MessageQueueTransaction();
            Message message = null;
            try
            {
                trans.Begin();
                message = queue.Receive(trans);
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

            action(queueName, message);
        }
    }
}
