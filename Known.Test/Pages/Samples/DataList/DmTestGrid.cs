using Known.Test.Pages.Samples.Models;

namespace Known.Test.Pages.Samples.DataList;

class DmTestGrid : DataGrid<DmGoods, DemoGoodsForm>
{
    public DmTestGrid()
    {
        Name = "测试示例";
    }

    protected override Task<PagingResult<DmGoods>> OnQueryData(PagingCriteria criteria)
    {
        return Task.FromResult(new PagingResult<DmGoods>
        {
            TotalCount = 220,
            PageData = GetGoodses(criteria)
        });
    }

    private static List<DmGoods> GetGoodses(PagingCriteria criteria)
    {
        var list = new List<DmGoods>();
        for (int i = 0; i < criteria.PageSize; i++)
        {
            var id = (criteria.PageIndex - 1) * criteria.PageSize + i;
            list.Add(DmGoods.RandomInfo(id));
        }
        return list;
    }
}