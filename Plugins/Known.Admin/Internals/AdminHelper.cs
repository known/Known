namespace Known.Internals;

class AdminHelper
{
    internal static async Task Install(Database db, InstallInfo info, SystemInfo sys)
    {
        await SaveCompanyAsync(db, info, sys);
    }

    private static async Task SaveCompanyAsync(Database db, InstallInfo info, SystemInfo sys)
    {
        var company = await db.QueryAsync<SysCompany>(d => d.Code == db.User.CompNo);
        company ??= new SysCompany();
        company.AppId = Config.App.Id;
        company.CompNo = info.CompNo;
        company.Code = info.CompNo;
        company.Name = info.CompName;
        company.SystemData = Utils.ToJson(sys);
        await db.SaveAsync(company);
    }
}