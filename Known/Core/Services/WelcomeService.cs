using System;
using System.Collections.Generic;

namespace Known.Core.Services
{
    public class WelcomeService : ServiceBase
    {
        public PagingResult<object> GetTodoLists(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 33,
                PageData = new List<object>
                {
                    new {Id="1",Name="请假审批",Qty=1,CreateTime=DateTime.Now},
                    new {Id="2",Name="费用报销",Qty=2,CreateTime=DateTime.Now},
                    new {Id="3",Name="出差审批",Qty=3,CreateTime=DateTime.Now}
                }
            };
        }

        public PagingResult<object> GetCompanyNews(PagingCriteria criteria)
        {
            return new PagingResult<object>
            {
                TotalCount = 55,
                PageData = new List<object>
                {
                    new {Id="1",Title="公司新系统上线",CreateBy="管理员",CreateTime=DateTime.Now},
                    new {Id="2",Title="关于放假通知",CreateBy="张三",CreateTime=DateTime.Now},
                    new {Id="3",Title="关于员工福利通知",CreateBy="李四",CreateTime=DateTime.Now}
                }
            };
        }

        internal PagingResult<object> GetShortCuts()
        {
            return null;
        }
    }
}
