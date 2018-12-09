using System.Collections.Generic;
using System.Web.Mvc;
using Known.Platform;
using Known.Platform.Services;
using Known.Web.Extensions;
using Known.Web.Models;
using Newtonsoft.Json;

namespace Known.Web.Controllers
{
    public class FrameController : AuthorizeController
    {
        private ModuleService Service
        {
            get { return LoadService<ModuleService>(); }
        }

        public ActionResult Index(string mid)
        {
            var module = Service.GetModule(mid);
            if (module == null)
                return Content("模块不存在！");

            switch (module.ViewType)
            {
                case Known.Platform.ViewType.DataGridView:
                    return View("DataGridView", new DataGridViewModel(module));
                case Known.Platform.ViewType.TreeGridView:
                    return View("TreeGridView", new TreeGridViewModel(module));
                case Known.Platform.ViewType.TabPageView:
                    return View("TabPageView", new TabPageViewModel(module));
                case Known.Platform.ViewType.SplitPageView:
                    return View("SplitPageView", new SplitPageViewModel(module));
                default:
                    return View(module);
            }
        }

        public ActionResult QueryDatas(string query, string isLoad)
        {
            var sortField = Request.Get<string>("sortField");
            var sortOrder = Request.Get<string>("sortOrder");
            var sorts = new List<string>();
            if (!string.IsNullOrWhiteSpace(sortField))
            {
                sorts.Add($"{sortField} {sortOrder}");
            }

            var criteria = new PagingCriteria
            {
                IsLoad = isLoad == "1",
                PageIndex = Request.Get<int>("pageIndex"),
                PageSize = Request.Get<int>("pageSize"),
                OrderBys = sorts.ToArray(),
                Parameter = FromJson(query)
            };

            var result = Service.QueryModules(criteria);
            return PageResult(result);
        }

        public ActionResult SaveData(string data)
        {
            var model = FromJson(data);
            var result = Service.SaveModule(model) as Result<Module>;
            return ExecuteResult(result);
        }

        public ActionResult DeleteDatas(string data)
        {
            return ErrorResult($"不能删除！{data}");
        }

        private dynamic FromJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            return JsonConvert.DeserializeObject<dynamic>(json);
        }
    }
}