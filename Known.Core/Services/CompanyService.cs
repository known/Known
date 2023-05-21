namespace Known.Core.Services;

public class CompanyService : BaseService
{
    private const string KeyCompany = "CompanyInfo";

    internal CompanyService(Context context) : base(context) { }

    //Company
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

    //Organization
    internal List<SysOrganization> GetOrganizations() => CompanyRepository.GetOrganizations(Database);

    internal Result DeleteOrganizations(List<SysOrganization> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var model in models)
        {
            if (CompanyRepository.ExistsSubOrganization(Database, model.Id))
                return Result.Error("存在子组织架构，不能删除！");
        }

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                db.Delete(item);
            }
        });
    }

    internal Result SaveOrganization(dynamic model)
    {
        var entity = Database.QueryById<SysOrganization>((string)model.Id);
        entity ??= new SysOrganization();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            if (CompanyRepository.ExistsOrganization(Database, entity))
                vr.AddError("组织编码已存在！");
        }
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
            PlatformHelper.SetBizOrganization(db, entity);
        }, entity);
    }
}