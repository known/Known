namespace Known.Core
{
    public class FlowActionInfo
    {
        public const string Open = "开启";
        public const string Over = "结束";
        public const string Stop = "终止";

        public string BizId { get; set; }
        public string BizStatus { get; set; }
        public string Note { get; set; }
    }

    public class FlowBizInfo
    {
        public string FlowCode { get; set; }
        public string FlowName { get; set; }
        public string BizId { get; set; }
        public string BizName { get; set; }
        public string BizUrl { get; set; }
        public string BizStatus { get; set; }
        public UserInfo CurrUser { get; set; }
    }
}