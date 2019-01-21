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
            var result = Service.DeleteApplyForms(new List<ApplyForm>());
            TestAssert.AreEqual(result.Message, "");
        }

        public static void CommitApplyForms()
        {
            var result = Service.CommitApplyForms(new List<ApplyForm>());
            TestAssert.AreEqual(result.Message, "");
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
            var form = new ApplyForm
            {
                Id = "",
                FormNo = "test",
                ApplyBy = "Known"
            };
            var model = form.ToJson().FromJson<dynamic>();
            var result = Service.SaveApplyForm(model) as Result;
            TestAssert.IsNotNull(result);
            TestAssert.AreEqual(result.Message, "");
        }
        #endregion
    }
}
