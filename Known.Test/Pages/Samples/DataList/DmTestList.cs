using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

class DmTestList : DataGrid<DmTest, DemoGoodsForm>
{
    public DmTestList()
    {
        Name = "测试示例";
    }

    protected override Task<PagingResult<DmTest>> OnQueryData(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<DmTest>
        {
            TotalCount = 220,
            PageData = GetTests(criteria)
        });
    }

    private static List<DmTest> GetTests(PagingCriteria criteria)
    {
        var list = new List<DmTest>();
        for (int i = 0; i < criteria.PageSize; i++)
        {
            var id = (criteria.PageIndex - 1) * criteria.PageSize + i;
            list.Add(new DmTest
            {
                No = $"{id}",
                Name = $"测试{criteria.PageIndex}-{i}",
                Title = $"规格型号{criteria.PageIndex}-{i}",
                Status = "个",
                Time = DateTime.Now,
                Picture = "/img/login.jpg",
                Icon = i % 2 == 0 ? "fa fa-toggle-on" : "fa fa-toggle-off",
                Color = "",
                Progress = 0.8M,
                Sort = i + 1,
                Enabled = true,
                Note = "测试哥哥是个大帅哥华盛顿国会山的"
            });
        }
        return list;
    }
}