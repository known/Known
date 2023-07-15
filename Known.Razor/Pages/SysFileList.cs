namespace Known.Razor.Pages;

class SysFileList : DataGrid<SysFile>
{
    public SysFileList()
    {
        OrderBy = $"{nameof(SysTask.CreateTime)} desc";
    }

    protected override Task<PagingResult<SysFile>> OnQueryData(PagingCriteria criteria)
    {
        return Platform.File.QueryFilesAsync(criteria);
    }
}