using AntDesign;
using Microsoft.AspNetCore.Components;

namespace Known.AntBlazor.Components;

public class AntSelect : Select<string, CodeInfo>
{
	[Parameter] public List<CodeInfo> Codes { get; set; }

	protected override void OnInitialized()
	{
        ValueName = nameof(CodeInfo.Code);
		LabelName = nameof(CodeInfo.Name);
        base.OnInitialized();
	}
}