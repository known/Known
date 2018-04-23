namespace Known.MsMq
{
    /// <summary>
    /// MQ配置信息
    /// </summary>
    public class MqConfigInfo
    {
        /// <summary>
        /// 取得或设置MQ名称。
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 取得或设置MQ地址。
        /// </summary>
        public string Path { get; set; }
    }
}