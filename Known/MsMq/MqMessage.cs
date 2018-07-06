using System;
using System.IO;

namespace Known.MsMq
{
    public class MqMessage
    {
        public string QueueName { get; set; }
        public string Label { get; set; }
        public Stream BodyStream { get; set; }
        public TimeSpan TimeToBeReceived { get; set; }
        public TimeSpan TimeToReachQueue { get; set; }
    }
}
