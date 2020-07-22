using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using Known.Web.Entities;

namespace Known.Web
{
    class DevelopService : ServiceBase
    {
        private IDevelopRepository Repository => Container.Resolve<IDevelopRepository>();
        private static readonly string path = $@"{Environment.CurrentDirectory}\domain.json";
        private static readonly List<DomainInfo> models = new List<DomainInfo>();

        static DevelopService()
        {
            var json = Utils.GetFileContent(path);
            var data = Utils.FromJson<List<DomainInfo>>(json);
            if (data != null && data.Count > 0)
                models.AddRange(data);
        }

        #region Module
        public PagingResult<SysModule> QueryModules(PagingCriteria criteria)
        {
            return Repository.QueryModules(Database, criteria);
        }

        public Result CopyModules(string[] ids, string mid)
        {
            var module = Database.QueryById<SysModule>(mid);
            if (module == null)
                return Result.Error("所选模块不存在！");

            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("复制", db =>
            {
                foreach (var item in entities)
                {
                    item.ParentId = module.Id;
                    db.Insert(item);
                }
            });
        }

        public Result DeleteModules(string[] ids)
        {
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            foreach (var item in entities)
            {
                if (Repository.ExistsSubModule(Database, item.Id))
                    return Result.Error($"{item.Name}存在子模块，不能删除！");
            }

            return Database.Transaction("删除", db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                    Repository.DeleteModuleRights(db, item.Id);
                }
            });
        }

        public byte[] ExportModules(string[] ids)
        {
            var entities = Database.QueryListById<SysModule>(ids);
            if (entities == null || entities.Count == 0)
                return null;

            entities = entities.OrderBy(e => e.ParentId).ThenBy(e => e.Sort).ToList();
            var date = DateTime.Now.ToString("yyyy-MM-dd");
            var sql = string.Empty;
            foreach (var item in entities)
            {
                sql += $@"
insert into SysModule(Id,CreateBy,CreateTime,Version,CompNo,ParentId,Type,Code,Name,Icon,Url,Sort,Enabled) values('{item.Id}','System','{date}',1,'{item.CompNo}','{item.ParentId}','{item.Type}','{item.Code}','{item.Name}','{item.Icon}','{item.Url}',{item.Sort},{item.Enabled})
GO";
            }

            return Encoding.UTF8.GetBytes(sql);
        }

        public List<SysModule> GetModules()
        {
            return Repository.GetModules(Database);
        }

        public SysModule GetModule(string id)
        {
            return Database.QueryById<SysModule>(id);
        }

        public Result SaveModule(dynamic model)
        {
            var entity = Database.QueryById<SysModule>((string)model.Id);
            if (entity == null)
                entity = new SysModule();

            entity.FillModel(model);
            var vr = entity.Validate();
            if (!vr.IsValid)
                return vr;

            Database.Save(entity);
            return Result.Success("保存成功！", entity.Id);
        }
        #endregion

        #region Code
        public List<DomainInfo> GetModels()
        {
            var lists = new List<DomainInfo>();
            var categories = models.Select(c => c.Category).Distinct();
            foreach (var item in categories)
            {
                lists.Add(new DomainInfo { Category = "", Code = item, Name = item });
            }
            lists.AddRange(models);
            return lists;
        }

        public Result GetModel(string code)
        {
            var model = models.FirstOrDefault(m => m.Code == code);
            if (model == null)
                return Result.Error("模型不存在！");

            var helper = new CodeHelper(model);
            return Result.Success("", new
            {
                Model = model,
                View = HttpUtility.HtmlEncode(helper.GetView()),
                Entity = HttpUtility.HtmlEncode(helper.GetEntity()),
                Controller = HttpUtility.HtmlEncode(helper.GetController()),
                Service = HttpUtility.HtmlEncode(helper.GetService()),
                Repository = HttpUtility.HtmlEncode(helper.GetRepository()),
                Sql = HttpUtility.HtmlEncode(helper.GetSql())
            });
        }

        public Result SaveModel(DomainInfo form)
        {
            if (form == null)
                return Result.Error("不能提交空数据！");

            var model = models.FirstOrDefault(m => m.Code == form.Code);
            if (model == null)
            {
                model = new DomainInfo();
                models.Add(model);
            }

            model.Category = form.Category;
            model.Code = form.Code;
            model.Name = form.Name;
            model.Fields = form.Fields;

            SaveModels();
            return Result.Success("保存成功！", new { Id = model.Code });
        }

        public Result DeleteModel(string code)
        {
            var model = models.FirstOrDefault(m => m.Code == code);
            if (model == null)
                return Result.Error("模型不存在！");

            models.Remove(model);
            SaveModels();
            return Result.Success("删除成功！");
        }

        private void SaveModels()
        {
            var json = Utils.ToJson(models);
            Utils.SaveFileContent(path, json);
        }
        #endregion
    }
}
