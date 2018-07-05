namespace Known
{
    /// <summary>
    /// 应用程序设定。
    /// </summary>
    public class Setting
    {
        private static readonly Setting instance = new Setting();

        private Setting() { }

        /// <summary>
        /// 取得应用程序设定实例。
        /// </summary>
        public Setting Instance
        {
            get { return instance; }
        }

        /// <summary>
        /// 取得是否启用数据修改前日志。
        /// </summary>
        public bool IsDbAudit { get; }
    }
}
