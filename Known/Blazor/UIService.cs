namespace Known.Blazor;

/// <summary>
/// UI服务接口。
/// </summary>
public interface IUIService
{
    /// <summary>
    /// 取得或设置多语言实例。
    /// </summary>
    Language Language { get; set; }

    /// <summary>
    /// 根据字段类型获取对应的输入组件类型。
    /// </summary>
    /// <param name="dataType">字段属性类型。</param>
    /// <param name="fieldType">字段类型。</param>
    /// <returns></returns>
    Type GetInputType(Type dataType, FieldType fieldType);

    /// <summary>
    /// 添加输入控件扩展参数。
    /// </summary>
    /// <typeparam name="TItem">表达数据类型。</typeparam>
    /// <param name="attributes">输入组件参数字段。</param>
    /// <param name="model">字段组件模型对象实例。</param>
    void AddInputAttributes<TItem>(Dictionary<string, object> attributes, FieldModel<TItem> model) where TItem : class, new();

    /// <summary>
    /// 弹出吐司组件提示消息框。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    Task Toast(string message, StyleType style = StyleType.Success);

    /// <summary>
    /// 弹出通知组件提示消息框。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="style">提示样式，默认Success。</param>
    /// <returns></returns>
    Task Notice(string message, StyleType style = StyleType.Success);

    /// <summary>
    /// 弹出消息提示框组件。
    /// </summary>
    /// <param name="message">提示消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    bool Alert(string message, Func<Task> action = null);

    /// <summary>
    /// 弹出确认消息提示框组件。
    /// </summary>
    /// <param name="message">确认消息文本。</param>
    /// <param name="action">点【确定】按钮的回调方法。</param>
    bool Confirm(string message, Func<Task> action);

    /// <summary>
    /// 弹出模态对话框组件。
    /// </summary>
    /// <param name="model">对话框组件模型对象。</param>
    bool ShowDialog(DialogModel model);

    /// <summary>
    /// 弹出表单组件对话框。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="model">表单组件模型对象。</param>
    bool ShowForm<TItem>(FormModel<TItem> model) where TItem : class, new();

    /// <summary>
    /// 构建表单组件。
    /// </summary>
    /// <typeparam name="TItem">表单数据类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表单组件模型对象。</param>
    void BuildForm<TItem>(RenderTreeBuilder builder, FormModel<TItem> model) where TItem : class, new();

    /// <summary>
    /// 构建按钮工具条组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">工具条组件模型对象。</param>
    void BuildToolbar(RenderTreeBuilder builder, ToolbarModel model);

    /// <summary>
    /// 构建表格查询表单组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格组件模型对象。</param>
    void BuildQuery(RenderTreeBuilder builder, TableModel model);

    /// <summary>
    /// 构建表格组件。
    /// </summary>
    /// <typeparam name="TItem">表格行数据类型。</typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">表格组件模型对象。</param>
    void BuildTable<TItem>(RenderTreeBuilder builder, TableModel<TItem> model) where TItem : class, new();

    /// <summary>
    /// 构建树组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">树组件模型对象。</param>
    void BuildTree(RenderTreeBuilder builder, TreeModel model);

    /// <summary>
    /// 构建步骤组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">步骤组件模型对象。</param>
    void BuildSteps(RenderTreeBuilder builder, StepModel model);

    /// <summary>
    /// 构建标签页组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">标签页组件模型对象。</param>
    void BuildTabs(RenderTreeBuilder builder, TabModel model);

    /// <summary>
    /// 构建下拉框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">下拉框组件模型对象。</param>
    void BuildDropdown(RenderTreeBuilder builder, DropdownModel model);

    /// <summary>
    /// 构建提示框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">提示文本。</param>
    /// <param name="type">提示框类型。</param>
    void BuildAlert(RenderTreeBuilder builder, string text, StyleType type = StyleType.Info);

    /// <summary>
    /// 构建标签组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="text">标签文本。</param>
    /// <param name="color">标签颜色。</param>
    void BuildTag(RenderTreeBuilder builder, string text, string color = null);

    /// <summary>
    /// 构建图标组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="type">图标类型。</param>
    /// <param name="onClick">图标单击事件。</param>
    void BuildIcon(RenderTreeBuilder builder, string type, EventCallback<MouseEventArgs>? onClick = null);

    /// <summary>
    /// 构建结果组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="status">结果状态。</param>
    /// <param name="message">结果描述信息。</param>
    void BuildResult(RenderTreeBuilder builder, string status, string message);

    /// <summary>
    /// 构建按钮组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="info">操作按钮信息对象。</param>
    void BuildButton(RenderTreeBuilder builder, ActionInfo info);

    /// <summary>
    /// 构建搜索框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildSearch(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建文本框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildText(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建多行文本框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildTextArea(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建密码框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildPassword(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建日期选择框组件。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildDatePicker<TValue>(RenderTreeBuilder builder, InputModel<TValue> model);

    /// <summary>
    /// 构建数字输入框组件。
    /// </summary>
    /// <typeparam name="TValue"></typeparam>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildNumber<TValue>(RenderTreeBuilder builder, InputModel<TValue> model);

    /// <summary>
    /// 构建复选框框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildCheckBox(RenderTreeBuilder builder, InputModel<bool> model);

    /// <summary>
    /// 构建开关组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildSwitch(RenderTreeBuilder builder, InputModel<bool> model);

    /// <summary>
    /// 构建下拉选择框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildSelect(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建单选框组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildRadioList(RenderTreeBuilder builder, InputModel<string> model);

    /// <summary>
    /// 构建复选框列表组件。
    /// </summary>
    /// <param name="builder">呈现树建造者。</param>
    /// <param name="model">输入模型对象。</param>
    void BuildCheckList(RenderTreeBuilder builder, InputModel<string[]> model);
}