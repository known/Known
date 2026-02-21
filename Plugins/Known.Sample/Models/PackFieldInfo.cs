namespace Known.Sample.Models;

public class PackFieldInfo
{
    public string Name { get; set; }
    public AppFieldType Type { get; set; }
    public WorkFieldType Work { get; set; }

    public string GetTypeName()
    {
        if (Work != WorkFieldType.None)
            return string.Empty;

        return Type.GetDescription();
    }

    public object GetValue(TbWork work)
    {
        if (work == null)
            return string.Empty;

        switch (Work)
        {
            case WorkFieldType.WorkNo: return work.WorkNo;
            case WorkFieldType.CustGNo: return work.CustGNo;
        }

        if (work.PackInfo.TryGetValue(Name, out object value))
            return value;

        return string.Empty;
    }
}

public enum AppFieldType
{
    [Display(Name = "文本框")]
    Text,
    [Display(Name = "整数框")]
    Integer,
    [Display(Name = "小数框")]
    Decimal,
    [Display(Name = "日期框")]
    DateTime
}

public enum WorkFieldType
{
    [Display(Name = "")]
    None,
    [Display(Name = "工单编号")]
    WorkNo,
    [Display(Name = "客户料号")]
    CustGNo
}