namespace Known.Core.Services;

class DictionaryService : BaseService
{
    internal DictionaryService(Context context) : base(context) { }

    internal Result RefreshCache() => RefreshCache(Database, CurrentUser);

    internal static Result RefreshCache(Database db, UserInfo user)
    {
        if (user == null)
            return Result.Error("用户未登录！");

        var entities = DictionaryRepository.GetDictionarys(db, user.AppId, user.CompNo);
        var codes = entities.Select(e =>
        {
            var code = e.Code;
            if (!string.IsNullOrWhiteSpace(e.Name))
                code = $"{code}-{e.Name}";
            return new CodeInfo(e.Category, code, code, e);
        }).ToList();
        var datas = PlatformHelper.Dictionary?.Invoke(db);
        if (datas != null && datas.Count > 0)
            codes.AddRange(datas);
        Cache.AttachCodes(codes);
        return Result.Success("刷新成功！", codes);
    }

    internal PagingResult<SysDictionary> QueryDictionarys(PagingCriteria criteria)
    {
        return DictionaryRepository.QueryDictionarys(Database, criteria);
    }

    internal Result DeleteDictionarys(List<SysDictionary> models)
    {
        if (models == null || models.Count == 0)
            return Result.Error(Language.SelectOneAtLeast);

        return Database.Transaction(Language.Delete, db =>
        {
            foreach (var item in models)
            {
                db.Delete(item);
            }
        });
    }

    internal Result SaveDictionary(dynamic model)
    {
        var entity = Database.QueryById<SysDictionary>((string)model.Id);
        entity ??= new SysDictionary();
        entity.FillModel(model);
        var vr = entity.Validate();
        if (!vr.IsValid)
            return vr;

        Database.Save(entity);
        RefreshCache();
        return Result.Success(Language.SaveSuccess, entity.Id);
    }
}