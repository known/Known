﻿namespace Known.Blazor;

/// <summary>
/// 对话框配置模型信息类。
/// </summary>
public class DialogModel
{
    /// <summary>
    /// 取得或设置对话框CSS类名。
    /// </summary>
    public string ClassName { get; set; }

    /// <summary>
    /// 取得或设置对话框标题。
    /// </summary>
    public string Title { get; set; }

    /// <summary>
    /// 取得或设置对话框是否可关闭，默认是。
    /// </summary>
    public bool Closable { get; set; } = true;

    /// <summary>
    /// 取得或设置对话框是否可拖动，默认是。
    /// </summary>
    public bool Draggable { get; set; } = true;

    /// <summary>
    /// 取得或设置对话框是否可调整大小。
    /// </summary>
    public bool Resizable { get; set; }

    /// <summary>
    /// 取得或设置对话框是否显示最大化按钮。
    /// </summary>
    public bool Maximizable { get; set; }

    /// <summary>
    /// 取得或设置对话框是否默认最大化显示。
    /// </summary>
    public bool DefaultMaximized { get; set; }

    /// <summary>
    /// 取得或设置对话框宽度。
    /// </summary>
    public double? Width { get; set; }

    /// <summary>
    /// 取得或设置对话框确定按钮事件委托方法。
    /// </summary>
    public Func<Task> OnOk { get; set; }

    /// <summary>
    /// 取得或设置对话框关闭委托。
    /// </summary>
    internal Func<Task> OnClose { get; set; }

    /// <summary>
    /// 取得或设置对话框关闭后调用的委托。
    /// </summary>
    public Action OnClosed { get; set; }

    /// <summary>
    /// 取得或设置对话框内容呈现模板。
    /// </summary>
    public RenderFragment Content { get; set; }

    /// <summary>
    /// 取得或设置对话框自定义底部模板。
    /// </summary>
    public RenderFragment Footer { get; set; }

    /// <summary>
    /// 异步关闭对话框。
    /// </summary>
    /// <returns></returns>
    public async Task CloseAsync()
    {
        if (OnClose != null)
            await OnClose.Invoke();

        OnClosed?.Invoke();
    }
}