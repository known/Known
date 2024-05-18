namespace Known;

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

    public static bool Enabled { get; set; }

    public static Stopwatcher Start<T>() => new(typeof(T));

    public void Watch(string label)
    {
        if (!Enabled)
            return;

        watch.Stop();
        times[label] = watch.ElapsedMilliseconds;
        watch.Restart();
    }

    public void Write(string label)
    {
        if (!Enabled)
            return;

        watch.Stop();
        Logger.Info($"{type.Name}：{label}={watch.ElapsedMilliseconds}ms");
        watch.Restart();
    }

    public void WriteLog()
    {
        if (times.Count == 0)
            return;

        var logs = string.Join("、", times.Select(t => $"{t.Key}={t.Value}ms"));
        Logger.Info($"{type.Name}：{logs}");
    }
}