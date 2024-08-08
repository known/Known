namespace Known.Repositories;

class FileRepository
{
    internal static Task<List<SysFile>> GetFilesAsync(Database db, string bizId)
    {
        return db.Query<SysFile>()
                 .Where(d => d.BizId == bizId)
                 .OrderBy(d => d.CreateTime)
                 .ToListAsync();
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