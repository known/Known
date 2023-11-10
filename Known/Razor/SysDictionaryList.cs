using Known.Entities;
using Known.Extensions;

namespace Known.Razor;

class SysDictionaryList : BasePage<SysDictionary>
{
    public SysDictionaryList()
    {
        //OrderBy = $"{nameof(SysDictionary.Sort)} asc";
    }

    protected override Task<PagingResult<SysDictionary>> OnQueryAsync(PagingCriteria criteria)
    {
        return Platform.Dictionary.QueryDictionarysAsync(criteria);
    }

    public void New() => Table.ShowForm(Platform.Dictionary.SaveDictionaryAsync, new SysDictionary());
    public void Edit(SysDictionary row) => Table.ShowForm(Platform.Dictionary.SaveDictionaryAsync, row);
    public void Delete(SysDictionary row) => Table.Delete(Platform.Dictionary.DeleteDictionarysAsync, row);
    public void DeleteM() => Table.DeleteM(Platform.Dictionary.DeleteDictionarysAsync);
    public void Import() => Table.ShowImportForm();
    public void Export()
    {
        UI.Info("暂未实现导出功能！");
    }
}