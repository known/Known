namespace Known.Core.Services;

public class CompanyService : BaseService
{
    private const string KeyCompany = "CompanyInfo";

    internal CompanyService(Context context) : base(context) { }

    public static string GetCompany(Database db, UserInfo user)
    {
        if (KCConfig.IsPlatform)
            return GetCompanyData(db, user);

        var model = PlatformRepository.GetConfig(db, Config.AppId, KeyCompany);
        if (string.IsNullOrEmpty(model))
            model = GetDefaultData(user);
        return model;
    }

    internal string GetCompany()
    {
        var user = CurrentUser;
        return GetCompany(Database, user);
    }

    internal Result SaveCompany(object model)
    {
        var user = CurrentUser;
        if (KCConfig.IsPlatform)
        {
            var company = CompanyRepository.GetCompany(Database, user.CompNo);
            if (company == null)
                return Result.Error("企业不存在！");

            company.CompanyData = Utils.ToJson(model);
            Database.Save(company);
        }
        else
        {
            SaveConfig(Database, KeyCompany, model);
        }
        return Result.Success("保存成功！");
    }

    private static string GetCompanyData(Database db, UserInfo user)
    {
        var company = CompanyRepository.GetCompany(db, user.CompNo);
        if (company == null)
            return GetDefaultData(user);

        var model = company.CompanyData;
        if (string.IsNullOrEmpty(model))
        {
            model = Utils.ToJson(new
            {
                company.Code,
                company.Name,
                company.NameEn,
                company.SccNo,
                company.Address,
                company.AddressEn
            });
        }
        return model;
    }

    private static string GetDefaultData(UserInfo user) => Utils.ToJson(new { Code = user.CompNo, Name = user.CompName });
}