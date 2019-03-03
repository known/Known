using System.Collections.Generic;
using Known.Core.Entities;
using Known.Extensions;

namespace Known.Core.Services
{
    public class DevDemoService : ServiceBase
    {
        public DevDemoService(Context context) : base(context)
        {
        }

        public PagingResult QueryUsers(PagingCriteria criteria)
        {
            var users = new List<User>();
            users.Add(new User
            {
                Id = "1",
                UserName = "admin",
                Name = "管理员",
                Email = "admin@known.com",
                Mobile = "18988888888",
                Phone = "68888888"
            });
            users.Add(new User
            {
                Id = "2",
                UserName = "zhangsan",
                Name = "张三",
                Email = "zhangsan@known.com"
            });
            for (int i = 3; i < 188; i++)
            {
                users.Add(new User
                {
                    Id = i.ToString(),
                    UserName = $"account{i}",
                    Name = $"操作员{i}"
                });
            }

            var data = users.ToPageList(criteria.PageIndex, criteria.PageSize);
            return new PagingResult(users.Count, data);
        }
    }
}
