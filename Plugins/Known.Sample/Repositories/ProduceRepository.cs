namespace Known.Sample.Repositories;

class ProduceRepository
{
    internal static async Task<bool> ExistsWorkMaterialAsync(Database db, string custGNo)
    {
        var sql = "select count(*) from TbWork where CompNo=@CompNo and CustGNo=@custGNo";
        return await db.ScalarAsync<int>(sql, new { db.User.CompNo, custGNo }) > 0;
    }

    internal static Task<TbWork> GetWorkAsync(Database db, string id)
    {
        var sql = @"
select a.*,b.PackFields 
from TbWork a 
left join TbMaterial b on b.CustGNo=a.CustGNo 
where a.Id=@id";
        return db.QueryAsync<TbWork>(sql, new { id });
    }
}