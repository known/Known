using System.Linq;
using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class DictionaryController : ControllerBase
    {
        private DictionaryService Service { get; } = new DictionaryService();

        #region View
        public ActionResult GetCategories()
        {
            var datas = Service.GetCategories();
            return JsonResult(datas.Select(d => new
            {
                id = d.Code,
                pid = "",
                title = $"{d.Sort}.{d.Name}({d.Code})",
                data = d
            }));
        }

        [HttpPost]
        public ActionResult QueryDictionarys(CriteriaData data)
        {
            return QueryPagingData(data, c => Service.QueryDictionarys(c));
        }

        [HttpPost]
        public ActionResult DeleteDictionarys(string data)
        {
            return PostAction<string[]>(data, d => Service.DeleteDictionarys(d));
        }
        #endregion

        #region Form
        public ActionResult GetDictionary(string id)
        {
            return JsonResult(Service.GetDictionary(id));
        }

        [HttpPost]
        public ActionResult SaveDictionary(string data)
        {
            return PostAction<dynamic>(data, d => Service.SaveDictionary(d));
        }
        #endregion
    }
}