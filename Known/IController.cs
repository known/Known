namespace Known
{
    /// <summary>
    /// 控制器接口。
    /// </summary>
    public interface IController
    {
        /// <summary>
        /// 取得上下文对象。
        /// </summary>
        Context Context { get; }

        /// <summary>
        /// 取得当前登录的用户账号。
        /// </summary>
        string UserName { get; }

        /// <summary>
        /// 取得当前用户是否已认证。
        /// </summary>
        bool IsAuthenticated { get; }

        /// <summary>
        /// 从对象容器中加载业务逻辑对象。
        /// </summary>
        /// <typeparam name="T">业务逻辑类型。</typeparam>
        /// <returns>业务逻辑对象。</returns>
        T LoadBusiness<T>() where T : BusinessBase;
    }
}
