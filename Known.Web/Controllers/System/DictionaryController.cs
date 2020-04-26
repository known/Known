using System.Web.Mvc;
using Known.Core.Services;

namespace Known.Web.Controllers
{
    public class DictionaryController : ControllerBase
    {
        private DictionaryService Service { get; } = new DictionaryService();

        #region View
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