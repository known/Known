using System.Diagnostics;

namespace Known;

/// <summary>
/// 性能秒表操作类。
/// </summary>
public class Stopwatcher
{
    private readonly Type type;
    private readonly Stopwatch watch;
    private readonly Dictionary<string, long> times = [];

    private Stopwatcher(Type type)
    {
        this.type = type;
        times.Clear();
        if (Enabled)
            watch = Stopwatch.StartNew();
    }

    /// <summary>
    /// 取得或设置是否启用秒表功能。
    /// </summary>
    public static bool Enabled { get; set; }

    /// <summary>
    /// 开始计时。
    /// </summary>
    /// <typeparam name="T">计时对象类型。</typeparam>
    /// <returns>秒表操作实例。</returns>
    public static Stopwatcher Start<T>() => new(typeof(T));

    /// <summary>
    /// 添加启停点，不打印日志。
    /// </summary>
    /// <param name="label">点位标题。</param>
    public void Watch(string label)
    {
        if (!Enabled)
            return;

        watch.Stop();
        times[label] = watch.ElapsedMilliseconds;
        watch.Restart();
    }

    /// <summary>
    /// 添加启停点，直接打印耗时时间日志。
    /// </summary>
    /// <param name="label">点位标题。</param>
    public void Write(string label)
    {
        if (!Enabled)
            return;

        watch.Stop();
        WriteLog($"{type.Name}：{label}={watch.ElapsedMilliseconds}ms");
        watch.Restart();
    }

    /// <summary>
    /// 打印所有Watch的时间日志。
    /// </summary>
    public void WriteLog()
    {
        if (times.Count == 0)
            return;

        var logs = string.Join("、", times.Select(t => $"{t.Key}={t.Value}ms"));
        WriteLog($"{type.Name}：{logs}");
    }

    private void WriteLog(string message)
    {
        Logger.Debug(LogTarget.Debug, new UserInfo { Name = nameof(Stopwatcher) }, message);
    }
}