namespace Known;

/// <summary>
/// 验证结果类。
/// </summary>
public class Result
{
    private readonly List<string> errors = [];
    private string message;

    /// <summary>
    /// 构造函数，创建一个验证结果类的实例。
    /// </summary>
    public Result()
    {
        errors.Clear();
    }

    private Result(string message, object data) : this()
    {
        this.message = message;
        Data = data;
    }

    /// <summary>
    /// 取得或设置操作成功后是否关闭对话框，默认关闭。
    /// </summary>
    public bool IsClose { get; set; } = true;

    /// <summary>
    /// 取得或设置操作是否成功。
    /// </summary>
    public bool IsValid { get; set; } = true;

    /// <summary>
    /// 取得或设置操作成功或失败提示消息。
    /// </summary>
    public string Message
    {
        get
        {
            if (errors.Count == 0)
                return message;

            return string.Join(Environment.NewLine, [.. errors]);
        }
        set { message = value; }
    }

    /// <summary>
    /// 取得或设置操作返回的扩展对象。
    /// </summary>
    public object Data { get; set; }

    /// <summary>
    /// 获取返回的扩展泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <returns>泛型对象。</returns>
    public T DataAs<T>()
    {
        if (Data == null)
            return default;

        if (Data is T data)
            return data;

        var dataString = Data.ToString();
        return Utils.FromJson<T>(dataString);
    }

    /// <summary>
    /// 添加错误信息。
    /// </summary>
    /// <param name="message">错误信息。</param>
    public void AddError(string message)
    {
        IsValid = false;
        errors.Add(message);
    }

    /// <summary>
    /// 验证条件是否通过，不通过则添加错误信息。
    /// </summary>
    /// <param name="broken">验证条件。</param>
    /// <param name="message">错误信息。</param>
    public void Validate(bool broken, string message)
    {
        if (broken)
        {
            AddError(message);
        }
    }

    /// <summary>
    /// 添加必填错误信息。
    /// </summary>
    /// <param name="context">上下文对象。</param>
    /// <param name="name">语言名称。</param>
    /// <param name="value">校验该值是否为空。</param>
    public void Required(Context context, string name, string value)
    {
        if (string.IsNullOrWhiteSpace(value))
            AddError(context.Language.Required(name));
    }

    /// <summary>
    /// 返回失败结果。
    /// </summary>
    /// <param name="message">错误信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>失败结果。</returns>
    public static Result Error(string message, object data = null)
    {
        var result = new Result("", data);
        result.AddError(message);
        return result;
    }

    /// <summary>
    /// 异步返回失败结果。
    /// </summary>
    /// <param name="message">错误信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>失败结果。</returns>
    public static Task<Result> ErrorAsync(string message, object data = null) => Task.FromResult(Error(message, data));

    /// <summary>
    /// 返回成功结果。
    /// </summary>
    /// <param name="message">成功信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>成功结果。</returns>
    public static Result Success(string message, object data = null) => new(message, data);

    /// <summary>
    /// 异步返回成功结果。
    /// </summary>
    /// <param name="message">成功信息。</param>
    /// <param name="data">扩展数据，默认为空。</param>
    /// <returns>成功结果。</returns>
    public static Task<Result> SuccessAsync(string message, object data = null)
    {
        return Task.FromResult(Success(message, data));
    }
}