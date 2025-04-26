using AntDesign;

namespace Known.Internals;

/// <summary>
/// 系统设置表单组件类。
/// </summary>
public class SettingTypeForm : Form<UserSettingInfo> { }

/// <summary>
/// 查询条件表单组件类。
/// </summary>
public class QueryDataForm : Form<Dictionary<string, QueryInfo>> { }

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

/// <summary>
/// 注册表单组件类。
/// </summary>
public class RegisterInfoForm : Form<RegisterFormInfo>
{
    /// <summary>
    /// 构造函数，创建一个注册表单组件类的实例。
    /// </summary>
    public RegisterInfoForm()
    {
        LabelCol = null;
        ValidateOnChange = true;
        ValidateMode = FormValidateMode.Rules;
    }
}

/// <summary>
/// 代码生产字段表单组件类。
/// </summary>
public class CodeFieldTypeForm : AntForm<CodeFieldInfo> { }

/// <summary>
/// 无代码表格字段表单组件类。
/// </summary>
public class PageColumnForm : AntForm<PageColumnInfo> { }

/// <summary>
/// 无代码表单字段表单组件类。
/// </summary>
public class FormFieldForm : AntForm<FormFieldInfo> { }