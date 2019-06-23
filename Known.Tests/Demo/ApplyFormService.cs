using System.Collections.Generic;
using System.Data;
using Known.Extensions;
using Known.Mapping;

namespace Known.Tests.Demo
{
    public class ApplyFormService : ServiceBase
    {
        public ApplyFormService(Context context) : base(context)
        {
        }

        #region View
        public PagingResult QueryApplyForms(PagingCriteria criteria)
        {
            return Database.QueryApplyForms(criteria);
        }

        public Result DeleteApplyForms(List<ApplyForm> entities)
        {
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("删除", db =>
            {
                entities.ForEach(e => db.Delete(e));
            });
        }

        public Result ImportApplyFormLists(ApplyForm form, DataTable data, Dictionary<int, string> errors)
        {
            if (data == null || data.Rows.Count == 0)
                return Result.Error("导入数据不能为空！");

            var duplicateKeys = data.GetDuplicateValues(new[] { "申请内容" });
            var entities = new List<ApplyFormList>();
            for (int i = 0; i < data.Rows.Count; i++)
            {
                var messages = new List<string>();
                var row = data.Rows[i];
                var key = $"{row["申请内容"]}";
                if (duplicateKeys.Contains(key))
                    messages.Add("导入文件中申请内容不能重复！");

                var entity = new ApplyFormList
                {
                    FormId = form.Id
                };
                entity.Content = Validator.ValidateNotEmpty<string>(messages, row, "申请内容");

                if (messages.Count > 0)
                {
                    errors.Add(i, string.Join(",", messages));
                    continue;
                }

                var validator = ValidateApplyFormList(entity);
                if (validator.HasError)
                {
                    errors.Add(i, validator.ErrorMessage);
                    continue;
                }

                entities.Add(entity);
            }

            if (errors.Count > 0)
                return Result.Error("导入校验失败！");

            return Database.Transaction("导入", db =>
            {
                entities.ForEach(e => db.Save(e));
            });
        }

        public Result CommitApplyForms(List<ApplyForm> entities)
        {
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Database.Transaction("提交", db =>
            {
                foreach (var item in entities)
                {
                    item.Status = ApplyStatus.Commit;
                    db.Save(item);
                }
            });
        }
        #endregion

        #region Form
        public ApplyForm GetApplyForm(string id)
        {
            var entity = Database.QueryById<ApplyForm>(id);
            if (entity != null)
            {
                entity.Lists = Database.GetApplyFormLists(entity.Id);
            }
            return entity;
        }

        public Result SaveApplyForm(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var entity = GetEntityById((string)model.Id, new ApplyForm());
            EntityHelper.FillModel(entity, model);

            var validator = ValidateApplyForm(entity);
            if (validator.HasError)
                return Result.Error(validator.ErrorMessage);

            entity.Lists = new List<ApplyFormList>();

            if (model.Lists != null && model.Lists.Count > 0)
            {
                foreach (var item in model.Lists)
                {
                    item.FormId = entity.Id;
                    entity.Lists.Add(item);
                }
            }

            return Database.Transaction("保存", db =>
            {
                entity.Lists.ForEach(l => db.Save(l));
                db.Save(entity);
            }, entity.Id);
        }
        #endregion

        #region Private
        private Validator ValidateApplyForm(ApplyForm entity)
        {
            var validator = EntityHelper.Validate(entity);
            return validator;
        }

        private Validator ValidateApplyFormList(ApplyFormList entity)
        {
            var validator = EntityHelper.Validate(entity);
            return validator;
        }
        #endregion
    }
}
