using AntDesign;

namespace Known.Blazor;

/// <summary>
/// 对象表单组件类。
/// </summary>
public class TypeForm : Form<object> { }

/// <summary>
/// 登录表单组件类。
/// </summary>
public class LoginInfoForm : Form<LoginFormInfo>
{
    /// <summary>
    /// 构造函数，创建一个登录表单组件类的实例。
    /// </summary>
    public LoginInfoForm()
    {
        LabelCol = null;
        ValidateOnChange = true;
        ValidateMode = FormValidateMode.Rules;
    }
}