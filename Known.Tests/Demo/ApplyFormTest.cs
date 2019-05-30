using System.Collections.Generic;
using System.Data;
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

            var form = new
            {
                Id = "",
                FormNo = "test",
                ApplyBy = "Known"
            };
            var model = form.ToJson().FromJson<dynamic>();
            result = Service.SaveApplyForm(model);
            TestAssert.AreEqual(result.Message, "操作成功！");
        }

        public static void ImportApplyFormLists()
        {
            var form = new ApplyForm();
            var data = new DataTable();
            data.Columns.Add("申请内容");

            var errors = new Dictionary<int, string>();
            var result = Service.ImportApplyFormLists(form, data, errors);
            TestAssert.AreEqual(result.Message, "导入数据不能为空！");

            for (int i = 0; i < 10; i++)
            {
                data.Rows.Add(i == 3 ? "" : $"内容{i}");
            }

            result = Service.ImportApplyFormLists(form, data, errors);
            TestAssert.AreEqual(errors.Count, 1);
            TestAssert.AreEqual(errors[3], "申请内容不能为空！");
        }
        #endregion
    }
}
