using Known.Sample.Entities;

namespace Known.Server;

class AppMigrate
{
    public static Task UpdateVersionAsync(Database db)
    {
        var time = DateTime.Parse("2026-02-21 15:00:00");
        return db.UpdateVersionAsync("UpdateTime", time, Update20260221Async);
    }

    private static async Task Update20260221Async(Database db)
    {
        await db.CreateTableAsync<TbMaterial>();
        await db.CreateTableAsync<TbWork>();
    }
}