namespace Known.Helpers;

class Snowflake(long machineId, long datacenterId)
{
    private long machineId = machineId; // 机器ID
    private long datacenterId = datacenterId; // 数据中心ID
    private long sequence = 0L; // 序列号
    private long twepoch = 1288834974657L; // 起始时间戳

    private static readonly long machineIdBits = 5L; // 机器ID的位数
    private static readonly long datacenterIdBits = 5L; // 数据中心ID的位数
    private static readonly long maxMachineId = -1L ^ (-1L << (int)machineIdBits); // 最大机器ID
    private static readonly long maxDatacenterId = -1L ^ (-1L << (int)datacenterIdBits); // 最大数据中心ID
    private static readonly long sequenceBits = 12L; // 序列号的位数

    // 各部分向左的位移
    private static readonly long machineIdShift = sequenceBits;
    private static readonly long datacenterIdShift = sequenceBits + machineIdBits;
    private static readonly long timestampLeftShift = sequenceBits + machineIdBits + datacenterIdBits;
    private static readonly long sequenceMask = -1L ^ (-1L << (int)sequenceBits);

    private long lastTimestamp = -1L;

    public long NextId()
    {
        lock (this)
        {
            var timestamp = TimeGen();
            if (timestamp < lastTimestamp)
                throw new Exception("Clock moved backwards.");

            if (lastTimestamp == timestamp)
            {
                sequence = (sequence + 1) & sequenceMask;
                if (sequence == 0)
                    timestamp = TilNextMillis(lastTimestamp);
            }
            else
            {
                sequence = 0;
            }

            lastTimestamp = timestamp;
            return ((timestamp - twepoch) << (int)timestampLeftShift) |
                   (datacenterId << (int)datacenterIdShift) |
                   (machineId << (int)machineIdShift) | sequence;
        }
    }

    private static long TilNextMillis(long lastTimestamp)
    {
        var timestamp = TimeGen();
        while (timestamp <= lastTimestamp)
        {
            timestamp = TimeGen();
        }
        return timestamp;
    }

    private static long TimeGen()
    {
        return (long)(DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc)).TotalMilliseconds;
    }
}