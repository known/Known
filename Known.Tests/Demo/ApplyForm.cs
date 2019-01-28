using System;
using System.Collections.Generic;
using Known.Mapping;

namespace Known.Tests.Demo
{
    public enum ApplyStatus
    {
        Save,
        Commit,
        Passed,
        Failed
    }

    public class ApplyForm : EntityBase
    {
        public string FormNo { get; set; }
        public string ApplyBy { get; set; }
        public DateTime ApplyTime { get; set; }
        public ApplyStatus Status { get; set; }
        public List<ApplyFormList> Lists { get; set; }
    }

    class ApplyFormEntityMapper : EntityMapper<ApplyForm>
    {
        public ApplyFormEntityMapper() :
            base("t_apply_form", "申请单")
        {
            this.Property(p => p.FormNo)
                .IsStringColumn("form_no", "申请单号", 1, 50, true);

            this.Property(p => p.ApplyBy)
                .IsStringColumn("apply_by", "申请人", 1, 50, false);

            this.Property(p => p.ApplyTime)
                .IsDateTimeColumn("apply_time", "申请时间", false);

            this.Property(p => p.Status)
                .IsEnumColumn("status", "状态");
        }
    }

    public class ApplyFormList : EntityBase
    {
        public string FormId { get; set; }
        public string Content { get; set; }
    }

    class ApplyFormListEntityMapper : EntityMapper<ApplyFormList>
    {
        public ApplyFormListEntityMapper() :
            base("t_apply_form_list", "申请单表体")
        {
            this.Property(p => p.FormId)
                .IsStringColumn("form_id", "申请单ID", 1, 50, true);

            this.Property(p => p.Content)
                .IsStringColumn("content", "申请内容", 1, 500, true);
        }
    }
}
