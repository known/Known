using System.Collections.Generic;
using System.Web.Http;
using Known.Extensions;
using Known.Platform;
using Known.Web;

namespace Known.WebApi.Controllers
{
    public class QueryParameter
    {
        public string Query { get; set; }
        public string IsLoad { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortField { get; set; }
        public string SortOrder { get; set; }
    }

    public class DemoController : BaseApiController
    {
        [HttpPost]
        [AllowAnonymous]
        public ApiResult QueryUsers(QueryParameter model)
        {
            var users = new List<User>();
            users.Add(new User
            {
                Id = "1",
                UserName = "admin",
                Name = "管理员",
                Email = "admin@known.com",
                Mobile = "18988888888",
                Phone = "68888888",
                Department = new Department
                {
                    Name = "研发中心"
                }
            });
            users.Add(new User
            {
                Id = "2",
                UserName = "zhangsan",
                Name = "张三",
                Email = "zhangsan@known.com",
                Mobile = "",
                Phone = "",
                Department = new Department
                {
                    Name = "管理中心"
                }
            });
            for (int i = 3; i < 188; i++)
            {
                users.Add(new User
                {
                    Id = i.ToString(),
                    UserName = $"account{i}",
                    Name = $"操作员{i}",
                    Department = new Department
                    {
                        Name = "操作部"
                    }
                });
            }

            var data = users.ToPageList(model.PageIndex, model.PageSize);
            return ApiResult.Success(new { total = 188, data });
        }
    }
}