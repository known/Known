using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Known.Web;

namespace Known.Core
{
    class PrototypeHelper
    {
        private static readonly Type serviceType = typeof(PrototypeService);
        private static readonly ServiceInfo service = new ServiceInfo(serviceType);

        internal static ActionInfo GetAction(string serviceName, string actionName)
        {
            MethodInfo method = null;
            if (actionName.StartsWith("query"))
                method = serviceType.GetMethod("Query");
            else if (actionName.StartsWith("delete"))
                method = serviceType.GetMethod("Delete");
            else if (actionName.StartsWith("copy"))
                method = serviceType.GetMethod("Copy");
            else if (actionName.StartsWith("save"))
                method = serviceType.GetMethod("Save");

            return new ActionInfo(service, method)
            {
                PrototypeName = GetPrototypeName(serviceName, actionName)
            };
        }

        private static string GetPrototypeName(string serviceName, string actionName)
        {
            var services = serviceName.Split('.');
            var name = actionName.Replace("ies", "y")
                                 .Replace("query", "")
                                 .Replace("delete", "")
                                 .Replace("copy", "")
                                 .Replace("save", "")
                                 .TrimEnd('s');
            return $"{services[0]}.{name}";
        }
    }

    class PrototypeService : ServiceBase, IService
    {
        class APrototype : EntityBase
        {
            public string Type { get; set; }
            public string HeadId { get; set; }
            public string Json { get; set; }
        }

        public PagingResult<Dictionary<string, object>> Query(PagingCriteria criteria)
        {
            var result = new PagingResult<Dictionary<string, object>>
            {
                PageData = new List<Dictionary<string, object>>()
            };
            var sql = "select * from APrototype where Type=@type order by CreateTime desc";
            var data = Database.QueryList<APrototype>(sql, new { type = PrototypeName });
            if (data.Count == 0)
            {
                result.TotalCount = 0;
                return result;
            }

            List<APrototype> heads = null;
            if (PrototypeName.EndsWith("list") || PrototypeName.EndsWith("item"))
            {
                sql = "select * from APrototype where Type=@type";
                var type = PrototypeName.Replace("list", "").Replace("item", "");
                heads = Database.QueryList<APrototype>(sql, new { type });
            }

            var datas = new List<Dictionary<string, object>>();
            foreach (var item in data)
            {
                var model = Utils.FromJson<Dictionary<string, object>>(item.Json);
                SetModelHead(heads, item, model);
                if (CheckModelQuery(criteria, model))
                {
                    model["Id"] = item.Id;
                    model["CreateBy"] = item.CreateBy;
                    model["CreateTime"] = item.CreateTime;
                    model["ModifyBy"] = item.ModifyBy;
                    model["ModifyTime"] = item.ModifyTime;
                    datas.Add(model);
                }
            }

            var pageDatas = datas.Skip((criteria.PageIndex.Value - 1) * criteria.PageSize.Value)
                                 .Take(criteria.PageSize.Value)
                                 .ToList();
            result.PageData.AddRange(pageDatas);
            result.TotalCount = datas.Count;
            return result;
        }

        private static bool CheckModelQuery(PagingCriteria criteria, Dictionary<string, object> model)
        {
            var condition = true;

            foreach (var para in criteria.Parameter)
            {
                if (model.ContainsKey(para.Key) && (!string.IsNullOrEmpty(para.Value) || para.Key == "HeadId"))
                {
                    var value = model[para.Key] == null ? "" : model[para.Key].ToString();
                    if (para.Key == "HeadId")
                        condition = condition && value == para.Value;
                    else
                        condition = condition && (value.Contains(para.Value) || $",{para.Value}".Contains($",{value}"));
                }
            }

            return condition;
        }

        private static void SetModelHead(List<APrototype> heads, APrototype item, Dictionary<string, object> model)
        {
            if (heads == null || heads.Count == 0)
                return;

            var head = heads.FirstOrDefault(h => h.Id == item.HeadId);
            if (head == null)
                return;

            var dicHead = Utils.FromJson<Dictionary<string, object>>(head.Json);
            foreach (var dic in dicHead)
            {
                if (dic.Key != "Id")
                {
                    model[dic.Key] = dic.Value;
                }
            }
        }

        public object Get(string id)
        {
            var entity = Database.QueryById<APrototype>(id);
            if (entity == null)
                return null;

            return Utils.ToDynamic(entity.Json);
        }

        public Result Delete(string data)
        {
            var ids = Utils.FromJson<string[]>(data);
            var entities = Database.QueryListById<APrototype>(ids);
            if (entities == null || entities.Count == 0)
                return Result.Error(Language.SelectOneAtLeast);

            return Database.Transaction(Language.Delete, db =>
            {
                foreach (var item in entities)
                {
                    db.Delete(item);
                }
            });
        }

        public Result Copy(string id)
        {
            var entity = Database.QueryById<APrototype>(id);
            if (entity == null)
                return Result.Error(Language.NotExists.Format("记录"));

            Database.Insert(entity);
            return Result.Success(Language.XXSuccess.Format(Language.Copy));
        }

        public Result Save(string data)
        {
            var model = Utils.ToDynamic(data);
            var entity = Database.QueryById<APrototype>((string)model.Id);
            if (entity == null)
            {
                entity = new APrototype { Type = PrototypeName };
                model.Id = entity.Id;
            }

            entity.HeadId = (string)model.HeadId;
            entity.Json = Utils.ToJson(model);
            Database.Save(entity);
            return Result.Success(Language.XXSuccess.Format(Language.Save), entity.Id);
        }
    }
}