using System;
using System.Diagnostics;
using System.Messaging;

namespace Known.MsMq
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
}
