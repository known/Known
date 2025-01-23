namespace Known.Blazor;

public partial class JSService
{
    /// <summary>
    /// 异步预览图片。
    /// </summary>
    /// <param name="inputElem">图片附件上传组件实例。</param>
    /// <param name="imgElem">图片预览Img控件实例。</param>
    /// <returns></returns>
    public Task PreviewImageAsync(ElementReference? inputElem, ElementReference imgElem)
    {
        return InvokeVoidAsync("KBlazor.previewImage", inputElem, imgElem);
    }

    /// <summary>
    /// 根据Img控件ID预览图片。
    /// </summary>
    /// <param name="inputElem">图片附件上传组件实例。</param>
    /// <param name="imgId">图片前端Img控件ID。</param>
    /// <returns></returns>
    public Task PreviewImageByIdAsync(ElementReference? inputElem, string imgId)
    {
        return InvokeVoidAsync("KBlazor.previewImageById", inputElem, imgId);
    }

    /// <summary>
    /// 异步绘制验证码组件。
    /// </summary>
    /// <param name="id">验证码控件ID。</param>
    /// <param name="code">验证码字符串。</param>
    /// <returns></returns>
    public Task CaptchaAsync(string id, string code)
    {
        return InvokeVoidAsync("KBlazor.captcha", id, code);
    }
}