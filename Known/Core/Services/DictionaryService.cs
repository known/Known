using System.Collections.Generic;
using Known.Core.Entities;
using Known.Core.Repositories;

namespace Known.Core.Services
{
    public class DictionaryService : ServiceBase
    {
        private IDictionaryRepository Repository { get; } = Container.Resolve<IDictionaryRepository>();

        #region View
        public List<SysDictionary> GetCategories()
        {
            return Repository.GetCategories(Database);
        }

        public PagingResult<SysDictionary> QueryDictionarys(PagingCriteria criteria)
        {
            return Repository.QueryDictionarys(Database, criteria);
        }

        public Result DeleteDictionarys(string[] ids)
        {
            var entities = Database.QueryListById<SysDictionary>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }
        #endregion

        #region Form
        public SysDictionary GetDictionary(string id)
        {
            return Database.QueryById<SysDictionary>(id);
        }

        public Result SaveDictionary(dynamic model)
        {
            var entity = Database.QueryById<SysDictionary>((string)model.Id);
            if (entity == null)
                entity = new SysDictionary();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion
    }
}
