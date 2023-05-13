namespace Known.Core.Repositories;

class FileRepository
{
    internal static PagingResult<SysFile> QueryFiles(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysFile where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPage<SysFile>(sql, criteria);
    }

    internal static bool HasFiles(Database db, string bizId)
    {
        var sql = "select count(*) from SysFile where BizId=@bizId";
        return db.Scalar<int>(sql, new { bizId }) > 0;
    }

    internal static List<SysFile> GetFiles(Database db, string bizId)
    {
        var sql = "select * from SysFile where BizId=@bizId order by CreateTime";
        return db.QueryList<SysFile>(sql, new { bizId });
    }

    internal static List<SysFile> GetFiles(Database db, string bizId, string bizType)
    {
        var sql = "select * from SysFile where BizId=@bizId and Type=@bizType order by CreateTime";
        return db.QueryList<SysFile>(sql, new { bizId, bizType });
    }

    internal static List<SysFile> GetFiles(Database db, string[] bizIds)
    {
        var idTexts = new List<string>();
        var paramters = new Dictionary<string, object>();
        for (int i = 0; i < bizIds.Length; i++)
        {
            idTexts.Add($"BizId=@id{i}");
            paramters.Add($"id{i}", bizIds[i]);
        }

        var idText = string.Join(" or ", idTexts.ToArray());
        var sql = $"select * from SysFile where {idText} order by CreateTime";
        return db.QueryList<SysFile>(sql, paramters);
    }
}