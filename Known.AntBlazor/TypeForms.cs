namespace Known.AntBlazor;

public class LoginInfoForm : Form<LoginFormInfo>
{
    public LoginInfoForm()
    {
        LabelCol = null;
        ValidateMode = FormValidateMode.Rules;
    }
}

public class QueryDataForm : Form<Dictionary<string, QueryInfo>> { }