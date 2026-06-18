using Known.Reports;

namespace Known.Sample.Pages.Produce;

[Route("/pms/reports")]
[Menu(AppConstant.Produce, "报表中心", "bar-chart", 3)]
public class ReportList : Report
{
    public override string SysId => "Test";
}