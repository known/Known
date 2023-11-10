using Known.Entities;

namespace Known.Repositories;

class FileRepository
{
    internal static Task<PagingResult<SysFile>> QueryFilesAsync(Database db, PagingCriteria criteria)
    {
        var sql = "select * from SysFile where AppId=@AppId and CompNo=@CompNo";
        return db.QueryPageAsync<SysFile>(sql, criteria);
    }

    internal static async Task<bool> HasFilesAsync(Database db, string bizId)
    {
        var sql = "select count(*) from SysFile where BizId=@bizId";
        return await db.ScalarAsync<int>(sql, new { bizId }) > 0;
    }

    internal static Task<List<SysFile>> GetFilesAsync(Database db, string bizId)
    {
        var sql = "select * from SysFile where BizId=@bizId order by CreateTime";
        return db.QueryListAsync<SysFile>(sql, new { bizId });
    }

    internal static Task<List<SysFile>> GetFilesAsync(Database db, string bizId, string bizType)
    {
        var sql = "select * from SysFile where BizId=@bizId and Type=@bizType order by CreateTime";
        return db.QueryListAsync<SysFile>(sql, new { bizId, bizType });
    }

    internal static Task<List<SysFile>> GetFilesAsync(Database db, string[] bizIds)
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
        return db.QueryListAsync<SysFile>(sql, paramters);
    }
}