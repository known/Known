namespace Known.Core.Services;

class OrganizationService : BaseService
{
    internal OrganizationService(Context context) : base(context) { }

    internal PagingResult<SysOrganization> QueryOrganizations(PagingCriteria criteria)
    {
        return OrganizationRepository.QueryOrganizations(Database, criteria);
    }

    internal List<SysOrganization> GetOrganizations(string appId, string compNo) => OrganizationRepository.GetOrganizations(Database, appId, compNo);
    internal SysOrganization GetOrganization(string appId, string compNo, string code) => OrganizationRepository.GetOrganization(Database, appId, compNo, code);

    internal Result DeleteOrganizations(List<SysOrganization> entities)
    {
        if (entities == null || entities.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        foreach (var item in entities)
        {
            if (OrganizationRepository.ExistsSubOrganization(Database, item.Id))
                return Result.Error($"{item.Name}存在子部门，不能删除！");
        }

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in entities)
            {
                db.Delete(item);
            }
        });
    }

    internal IEnumerable<object> GetOrganizations()
    {
        var user = CurrentUser;
        var datas = OrganizationRepository.GetOrganizations(Database, user.AppId, user.CompNo);
        datas ??= new List<SysOrganization>();
        return datas.Select(d => d.ToTree());
    }

    internal Result SaveOrganization(string data)
    {
        var user = CurrentUser;
        var model = Utils.ToDynamic(data);
        var entity = Database.QueryById<SysOrganization>((string)model.Id);
        if (entity == null)
        {
            entity = new SysOrganization
            {
                CompNo = user.CompNo,
                AppId = user.AppId
            };
        }

        entity.FillModel(model);
        var vr = ValidateOrganization(Database, entity);
        if (!vr.IsValid)
            return vr;

        return Database.Transaction(Language.Save, db =>
        {
            db.Save(entity);
            PlatformHelper.SetBizOrganization(db, user, entity);
        }, entity.Id);
    }

    private static Result ValidateOrganization(Database db, SysOrganization entity)
    {
        var vr = entity.Validate();
        if (vr.IsValid)
        {
            if (OrganizationRepository.ExistsOrganization(db, entity))
                vr.AddError("组织编码已存在！");
        }

        return vr;
    }
}