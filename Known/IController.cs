namespace Known
{
    /// <summary>
    /// 控制器接口。
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 取得当前登录的用户账号。
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 取得当前用户是否已认证。
        /// </summary>
        bool IsAuthenticated { get; }
    }
}
