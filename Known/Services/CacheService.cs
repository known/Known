namespace Known.Services;

class CacheService<T>
{
    internal static readonly ConcurrentDictionary<string, T> cached = new();
    private static readonly ConcurrentDictionary<string, Timer> timers = new();

    public ICollection<T> Values => cached.Values;

    public void Clear()
    {
        foreach (var timer in timers.Values)
        {
            timer.Dispose();
        }
        cached.Clear();
        timers.Clear();
    }

    public T Get(string key)
    {
        if (string.IsNullOrWhiteSpace(key))
            return default;

        if (!cached.TryGetValue(key, out T value))
            return default;

        return value;
    }

    public void Remove(string key)
    {
        if (string.IsNullOrEmpty(key))
            return;

        cached.TryRemove(key, out _);
        if (timers.TryRemove(key, out Timer timer))
        {
            timer.Dispose();
        }
    }

    public void Set(string key, T value, TimeSpan? timeSpan = null)
    {
        if (string.IsNullOrEmpty(key))
            return;

        cached[key] = value;

        if (timeSpan != null)
        {
            if (timers.TryGetValue(key, out Timer existingTimer))
                existingTimer.Dispose();

            var timer = new Timer(RemoveCacheItem, key, timeSpan.Value, Timeout.InfiniteTimeSpan);
            timers[key] = timer;
        }
    }

    private void RemoveCacheItem(object state)
    {
        var key = (string)state;
        Remove(key);
    }
}