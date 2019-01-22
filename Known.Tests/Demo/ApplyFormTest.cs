using System.Collections.Generic;
using Known.Extensions;

namespace Known.Tests.Demo
{
    public class ApplyFormTest
    {
        private static ApplyFormService Service
        {
            get { return Container.Resolve<ApplyFormService>(); }
        }

        #region Index
        public static void QueryApplyForms()
        {
            var result = Service.QueryApplyForms(new PagingCriteria
            {
            });
            TestAssert.AreEqual(result.TotalCount, 138);
        }

        public static void DeleteApplyForms()
        {
            var result = Service.DeleteApplyForms(null);
            TestAssert.AreEqual(result.Message, "请至少选择一条记录进行操作！");

            result = Service.DeleteApplyForms(new List<ApplyForm>());
            TestAssert.AreEqual(result.Message, "请至少选择一条记录进行操作！");
        }

        public static void CommitApplyForms()
        {
            var result = Service.CommitApplyForms(null);
            TestAssert.AreEqual(result.Message, "请至少选择一条记录进行操作！");

            result = Service.CommitApplyForms(new List<ApplyForm>());
            TestAssert.AreEqual(result.Message, "请至少选择一条记录进行操作！");
        }
        #endregion

        #region Form
        public static void GetApplyForm()
        {
            var entity = Service.GetApplyForm("");
            TestAssert.IsNull(entity);
        }

        public static void SaveApplyForm()
        {
            var result = Service.SaveApplyForm(null);
            TestAssert.AreEqual(result.Message, "不能提交空数据！");

            var form = new ApplyForm
            {
                Id = "",
                FormNo = "test",
                ApplyBy = "Known"
            };
            var model = form.ToJson().FromJson<dynamic>();
            result = Service.SaveApplyForm(model);
            TestAssert.IsNotNull(result);
            TestAssert.AreEqual(result.Message, "操作成功！");
        }
        #endregion
    }
}
