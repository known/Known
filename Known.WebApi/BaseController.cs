﻿using System.Web;
using System.Web.Http;
using Known.Log;

namespace Known.WebApi
{
    /// <summary>
    /// 控制器基类。
    /// </summary>
    public class BaseController : ApiController
    {
        private Context context;

        /// <summary>
        /// 取得上下文对象。
        /// </summary>
        public Context Context
        {
            get
            {
                if (context == null)
                {
                    var database = Config.GetDatabase();
                    var logger = new TraceLogger(HttpRuntime.AppDomainAppPath);
                    context = new Context(database, logger, UserName);
                }
                return context;
            }
        }

        /// <summary>
        /// 取得当前登录的用户账号。
        /// </summary>
        protected string UserName
        {
            get { return User.Identity.Name; }
        }

        /// <summary>
        /// 取得当前用户是否已认证。
        /// </summary>
        protected bool IsAuthenticated
        {
            get { return User.Identity.IsAuthenticated; }
        }

        /// <summary>
        /// 从对象容器中加载业务逻辑对象。
        /// </summary>
        /// <typeparam name="T">业务逻辑类型。</typeparam>
        /// <returns>业务逻辑对象。</returns>
        protected T LoadBusiness<T>() where T : Business
        {
            return BusinessFactory.Create<T>(Context);
        }
    }
}