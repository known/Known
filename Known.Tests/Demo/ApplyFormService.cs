using System.Collections.Generic;
using Known.Mapping;

namespace Known.Tests.Demo
{
    public class ApplyFormService : ServiceBase<IApplyFormRepository>
    {
        public ApplyFormService(Context context) : base(context)
        {
        }

        #region Index
        public PagingResult QueryApplyForms(PagingCriteria criteria)
        {
            return Repository.QueryApplyForms(criteria);
        }

        public Result DeleteApplyForms(List<ApplyForm> entities)
        {
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Repository.Transaction(rep =>
            {
                entities.ForEach(e => rep.Delete(e));
            });
        }

        public Result CommitApplyForms(List<ApplyForm> entities)
        {
            if (entities == null || entities.Count == 0)
                return Result.Error("请至少选择一条记录进行操作！");

            return Repository.Transaction(rep =>
            {
                foreach (var item in entities)
                {
                    item.Status = ApplyStatus.Commit;
                    rep.Save(item);
                }
            });
        }
        #endregion

        #region Form
        public ApplyForm GetApplyForm(string id)
        {
            var entity = Repository.QueryById<ApplyForm>(id);
            if (entity != null)
            {
                entity.Lists = Repository.GetApplyFormLists(entity.Id);
            }
            return entity;
        }

        public Result SaveApplyForm(dynamic model)
        {
            if (model == null)
                return Result.Error("不能提交空数据！");

            var entity = GetEntityById((string)model.Id, new ApplyForm());
            EntityHelper.FillModel(entity, model);

            var validator = EntityHelper.Validate(entity);
            var vr = validator.ToResult();
            if (vr.HasError)
                return Result.Error(vr.ErrorMessage);

            if (model.Lists != null && model.Lists.Count > 0)
            {
                entity.Lists = new List<ApplyFormList>();
                foreach (var item in model.Lists)
                {
                    item.FormId = entity.Id;
                    entity.Lists.Add(item);
                }
            }

            return Repository.Transaction(rep =>
            {
                if (entity.Lists != null && entity.Lists.Count > 0)
                {
                    entity.Lists.ForEach(l => rep.Save(l));
                }
                rep.Save(entity);
            }, entity.Id);
        }
        #endregion
    }
}
