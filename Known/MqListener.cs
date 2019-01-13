using System;
using System.Diagnostics;
using System.IO;
using System.Messaging;
using System.Text;

namespace Known
{
    public class MqListener
    {
        private bool listen;
        private MessageQueue queue;
        private Action<MqMessage> action;

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

        public MqConfigInfo Config { get; }

        public void Start(Action<MqMessage> action)
        {
            this.action = action;
            listen = true;
            queue.PeekCompleted += new PeekCompletedEventHandler(OnPeekCompleted);
            queue.ReceiveCompleted += new ReceiveCompletedEventHandler(OnReceiveCompleted);
            StartListening();
        }

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

    public class MqConfigInfo
    {
        public string Name { get; set; }
        public string Path { get; set; }
    }

    public class MqMessage
    {
        public string QueueName { get; set; }
        public string Label { get; set; }
        public Stream BodyStream { get; set; }
        public TimeSpan TimeToBeReceived { get; set; }
        public TimeSpan TimeToReachQueue { get; set; }
    }

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
