using System;
using System.IO;

namespace Known.MsMq
{
    /// <summary>
    /// MQ消息。
    /// </summary>
    public class MqMessage
    {
        /// <summary>
        /// 取得或设置MQ队列名称。
        /// </summary>
        public string QueueName { get; set; }

        /// <summary>
        /// 取得或设置消息标签。
        /// </summary>
        public string Label { get; set; }

        /// <summary>
        /// 取得或设置消息内容流。
        /// </summary>
        public Stream BodyStream { get; set; }

        /// <summary>
        /// 取得或设置接收消息时间。
        /// </summary>
        public TimeSpan TimeToBeReceived { get; set; }

        /// <summary>
        /// 取得或设置消息到达队列时间。
        /// </summary>
        public TimeSpan TimeToReachQueue { get; set; }
    }
}
