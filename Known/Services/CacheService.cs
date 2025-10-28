namespace Known.Services;

class CacheService<T>
{
    internal static readonly ConcurrentDictionary<object, T> cached = new();
    private static readonly ConcurrentDictionary<object, Timer> timers = new();

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

    public T Get(object key)
    {
        if (key == null)
            return default;

        if (!cached.TryGetValue(key, out T value))
            return default;

        return value;
    }

    public void Remove(object key)
    {
        if (key == null)
            return;

        cached.TryRemove(key, out _);
        if (timers.TryRemove(key, out Timer timer))
        {
            timer.Dispose();
        }
    }

    public void Set(object key, T value, TimeSpan? timeSpan = null)
    {
        if (key == null)
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

    private void RemoveCacheItem(object key)
    {
        Remove(key);
    }
}