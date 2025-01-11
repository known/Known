using AntDesign;

namespace Known.Components;

/// <summary>
/// 自定义Ant验证码组件类。
/// </summary>
public class AntCaptcha : Input<string>
{
    private KCaptcha captcha;

    /// <summary>
    /// 构造函数，创建一个自定义Ant验证码组件类的实例。
    /// </summary>
    public AntCaptcha()
    {
        Class = "ant-captcha";
    }

    /// <summary>
    /// 取得或设置前缀图标。
    /// </summary>
    [Parameter] public string Icon { get; set; }

    /// <summary>
    /// 取得或设置验证码组件选项实例。
    /// </summary>
    [Parameter] public CaptchaOption Option { get; set; }

    /// <summary>
    /// 验证码验证方法。
    /// </summary>
    /// <param name="message">验证结果提示信息。</param>
    /// <returns></returns>
    public bool Validate(out string message) => captcha.Validate(Value, out message);

    /// <summary>
    /// 刷新验证码。
    /// </summary>
    public void Refresh() => captcha.Refresh();

    /// <inheritdoc />
    protected override void OnInitialized()
    {
        if (!string.IsNullOrWhiteSpace(Icon))
            Prefix = b => b.Icon(Icon);
        base.OnInitialized();
    }

    /// <inheritdoc />
    protected override void BuildRenderTree(RenderTreeBuilder builder)
    {
        base.BuildRenderTree(builder);
        builder.Component<KCaptcha>().Set(c => c.Option, Option).Build(value => captcha = value);
    }
}