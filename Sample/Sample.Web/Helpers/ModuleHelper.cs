namespace Sample.Web.Helpers;

class ModuleHelper
{
    internal static void InitAppModules()
    {
        var modules = new List<SysModule>();
        var bizApply = GetModule("BizApply", "业务申请", "appstore", ModuleType.Menu.ToString(), 2);
        modules.Add(bizApply);
        modules.Add(GetBaApplyList(bizApply.Id));
        modules.Add(GetBaVerifyList(bizApply.Id));
        modules.Add(GetBaQueryList(bizApply.Id));

        Config.AppModules = modules;
    }

    private static SysModule GetModule(string code, string name, string icon, string target, int sort)
    {
        return new SysModule { ParentId = "0", Code = code, Name = name, Icon = icon, Target = target, Sort = sort, Enabled = true };
    }

    private static SysModule GetBaApplyList(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "BaApplyList",
            Name = "表单申请",
            Icon = "form",
            Description = "查询和维护业务申请单。",
            Target = ModuleType.Page.ToString(),
            Url = "/bas/applies",
            Sort = 1,
            Enabled = true,
            EntityData = @"申请单|TbApply|Y
业务类型|BizType|Text|50|Y
业务单号|BizNo|Text|50|Y
业务名称|BizTitle|Text|100|Y
业务内容|BizContent|TextArea
业务附件|BizFile|File|250",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":\"1000\",\"FixedHeight\":null,\"Tools\":[\"New\",\"DeleteM\"],\"Actions\":[\"Edit\",\"Delete\",\"Submit\",\"Revoke\"],\"Columns\":[{\"Id\":\"BizType\",\"Name\":\"业务类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":true,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"BizStatus\",\"Name\":\"流程状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"ApplyBy\",\"Name\":\"申请人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"ApplyTime\",\"Name\":\"申请时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"180\"},{\"Id\":\"VerifyBy\",\"Name\":\"审核人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"100\"},{\"Id\":\"VerifyTime\",\"Name\":\"审核时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"180\"},{\"Id\":\"VerifyNote\",\"Name\":\"审核意见\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":true,\"DefaultSort\":null,\"Fixed\":null,\"Width\":\"200\"}]}",
            FormData = "{\"Width\":800,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":true,\"MultiFile\":false,\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizContent\",\"Name\":\"业务内容\",\"Type\":1,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":true,\"Id\":\"BizFile\",\"Name\":\"业务附件\",\"Type\":10,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetBaVerifyList(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "BaVerifyList",
            Name = "表单审核",
            Icon = "ordered-list",
            Description = "查询和审核业务申请单。",
            Target = ModuleType.Page.ToString(),
            Url = "/bas/verifies",
            Sort = 2,
            Enabled = true,
            EntityData = "TbApply",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"ScrollX\":null,\"ScrollY\":null,\"Tools\":null,\"Actions\":[\"Verify\"],\"Columns\":[{\"Id\":\"BizType\",\"Name\":\"业务类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":true,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"BizStatus\",\"Name\":\"流程状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"ApplyBy\",\"Name\":\"申请人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"ApplyTime\",\"Name\":\"申请时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"VerifyBy\",\"Name\":\"审核人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"VerifyTime\",\"Name\":\"审核时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null},{\"Id\":\"VerifyNote\",\"Name\":\"审核意见\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":null}]}",
            FormData = "{\"Width\":800,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":true,\"MultiFile\":false,\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizContent\",\"Name\":\"业务内容\",\"Type\":1,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizFile\",\"Name\":\"业务附件\",\"Type\":10,\"Length\":null,\"Required\":false}]}"
        };
    }

    private static SysModule GetBaQueryList(string parentId)
    {
        return new SysModule
        {
            ParentId = parentId,
            Code = "BaQueryList",
            Name = "表单查询",
            Icon = "unordered-list",
            Description = "查询所有已审核的业务申请单。",
            Target = ModuleType.Page.ToString(),
            Url = "/bas/queries",
            Sort = 3,
            Enabled = true,
            EntityData = "TbApply",
            PageData = "{\"Type\":null,\"ShowPager\":true,\"FixedWidth\":null,\"FixedHeight\":null,\"Tools\":[\"Export\",\"Reapply\"],\"Actions\":[\"Print\"],\"Columns\":[{\"Id\":\"BizType\",\"Name\":\"业务类型\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":true,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"IsViewLink\":true,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"BizStatus\",\"Name\":\"流程状态\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"ApplyBy\",\"Name\":\"申请人\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"ApplyTime\",\"Name\":\"申请时间\",\"IsViewLink\":false,\"IsQuery\":true,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"VerifyBy\",\"Name\":\"审核人\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":100,\"Align\":null},{\"Id\":\"VerifyTime\",\"Name\":\"审核时间\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null},{\"Id\":\"VerifyNote\",\"Name\":\"审核意见\",\"IsViewLink\":false,\"IsQuery\":false,\"IsQueryAll\":false,\"IsSum\":false,\"IsSort\":false,\"DefaultSort\":null,\"Fixed\":null,\"Width\":150,\"Align\":null}]}",
            FormData = "{\"Width\":800,\"Maximizable\":false,\"DefaultMaximized\":false,\"LabelSpan\":null,\"WrapperSpan\":null,\"Fields\":[{\"Row\":1,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":true,\"MultiFile\":false,\"Id\":\"BizNo\",\"Name\":\"业务单号\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":1,\"Column\":2,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizTitle\",\"Name\":\"业务名称\",\"Type\":0,\"Length\":null,\"Required\":true},{\"Row\":2,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizContent\",\"Name\":\"业务内容\",\"Type\":1,\"Length\":null,\"Required\":false},{\"Row\":3,\"Column\":1,\"CategoryType\":null,\"Category\":null,\"Placeholder\":null,\"ReadOnly\":false,\"MultiFile\":false,\"Id\":\"BizFile\",\"Name\":\"业务附件\",\"Type\":10,\"Length\":null,\"Required\":false}]}"
        };
    }
}