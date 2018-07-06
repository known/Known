namespace Known
{
    public class Setting
    {
        private static readonly Setting instance = new Setting();

        private Setting() { }

        public Setting Instance
        {
            get { return instance; }
        }

        public bool IsDbAudit { get; }
    }
}
