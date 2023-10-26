using Known.Demo.Pages.Samples.Models;

namespace Known.Demo.Pages.Samples.DataList;

class DmTestList : KDataGrid<DmTest, TestForm>
{
    public DmTestList()
    {
        Name = "测试示例";
    }

    protected override Task<PagingResult<DmTest>> OnQueryDataAsync(PagingCriteria criteria)
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
            list.Add(DmTest.RandomInfo(id));
        }
        return list;
    }
}