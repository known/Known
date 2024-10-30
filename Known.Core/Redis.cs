using System.Net.Sockets;

namespace Known.Core;

enum RedisCmd
{
    GET,
    INFO,
    SET,
    EXPIRE,
    MULTI,
    EXEC,
    QUIT,
    SUBSCRIBE,
    UNSUBSCRIBE,
    PSUBSCRIBE,
    PUNSUBSCRIBE,
    PUBLISH,
    PUBSUB,
    AUTH,
    PING,
    DBSIZE,
    DEL,
    SELECT
}

class RedisReply
{
    internal const string Head = "*{0}\r\n";
    internal const string Argument = "${0}\r\n{1}\r\n";
    internal const char CR = '\r';
    internal const char LF = '\n';
    internal const char Error = '-';     //- 错误
    internal const char Status = '+';    //+ 字符串
    internal const char Bulk = '$';      //$ Bulk Strings
    internal const char MultiBulk = '*'; //* 数组
    internal const char Figure = ':';    //: 整数
    internal const string Success = "OK";
    internal const string Pong = "PONG";
}

/// <summary>
/// Redis客户端类。
/// </summary>
/// <param name="host">Redis主机地址，默认localhost。</param>
/// <param name="port">Redis主机端口，默认6379。</param>
public class RedisClient(string host = "localhost", int? port = 6379) : IDisposable
{
    private readonly string host = host;
    private readonly int port = port.Value;
    private Socket socket;
    private readonly byte[] buffer = new byte[100000];

    /// <summary>
    /// 取得或设置访问密码。
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 释放Redis客户端连接。
    /// </summary>
    public virtual void Dispose() => Close();

    /// <summary>
    /// Ping服务器。
    /// </summary>
    /// <returns></returns>
    public bool Ping() => ExecuteCommand(RedisCmd.PING).Equals(RedisReply.Pong);

    /// <summary>
    /// 选择Redis服务器。
    /// </summary>
    /// <returns></returns>
    public bool Select() => ExecuteCommand(RedisCmd.SELECT).Equals(RedisReply.Success);

    /// <summary>
    /// 获取服务器版本信息。
    /// </summary>
    /// <returns></returns>
    public string GetServerInfo() => ExecuteCommand(RedisCmd.INFO).ToString();

    /// <summary>
    /// 获取缓存泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <returns>泛型对象。</returns>
    public T Get<T>(string key)
    {
        var result = ExecuteCommand(RedisCmd.GET, key);
        if (result == null)
            return default;

        var json = result.ToString();
        return Utils.FromJson<T>(json);
    }

    /// <summary>
    /// 设置缓存泛型对象。
    /// </summary>
    /// <typeparam name="T">泛型类型。</typeparam>
    /// <param name="key">缓存键。</param>
    /// <param name="value">泛型对象。</param>
    /// <param name="expire">过期时长。</param>
    /// <returns>返回是否设置成功。</returns>
    public bool Set<T>(string key, T value, int? expire = null)
    {
        var json = Utils.ToJson(value);

        if (expire.HasValue)
        {
            ExecuteCommand(RedisCmd.MULTI);
            ExecuteCommand(RedisCmd.SET, key, json);
            ExecuteCommand(RedisCmd.EXPIRE, key, expire.Value.ToString());
            var result = ExecuteCommand(RedisCmd.EXEC) as object[];
            return result[0].Equals(RedisReply.Success);
        }

        return ExecuteCommand(RedisCmd.SET, key, json).Equals(RedisReply.Success);
    }

    /// <summary>
    /// 删除缓存对象。
    /// </summary>
    /// <param name="key">缓存键。</param>
    /// <returns></returns>
    /// <exception cref="Exception">未应答异常。</exception>
    public int Delete(string key)
    {
        var reply = ExecuteCommand(RedisCmd.DEL, key).ToString();
        if (int.TryParse(reply, out int nums))
            return nums;

        throw new Exception(reply);
    }

    private void Connect()
    {
        socket ??= new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        if (socket.Connected)
            return;

        socket.Connect(host, port);
    }

    private void Close()
    {
        var status = socket.Connected;

        try
        {
            if (status)
                ExecuteCommand(RedisCmd.QUIT);
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }

        try
        {
            if (status)
                socket.Shutdown(SocketShutdown.Both);
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }

        try
        {
            if (socket != null)
                socket.Close();
            socket = null;
        }
        catch (Exception ex)
        {
            Debug.Write(ex.ToString());
        }
    }

    private object ExecuteCommand(RedisCmd cmd, params string[] args)
    {
        Connect();
        if (!string.IsNullOrEmpty(Password))
        {
            SendData(RedisCmd.AUTH, Password);
            ReadData();
        }
        SendData(cmd, args);
        return ReadData();
    }

    private void SendData(RedisCmd cmd, params string[] args)
    {
        var bulk = RedisReply.Argument;
        var cmdStr = cmd.ToString();

        var sb = new StringBuilder();
        sb.AppendFormat(RedisReply.Head, args.Length + 1);
        sb.AppendFormat(bulk, cmdStr.Length, cmdStr);

        foreach (var arg in args)
        {
            sb.AppendFormat(bulk, arg.Length, arg);
        }

        var content = Encoding.UTF8.GetBytes(sb.ToString());
        socket.Send(content);
    }

    private object ReadData()
    {
        var b = (char)ReadFirstByte();
        if (b == RedisReply.MultiBulk)
            return ReadMultiBulk();
        else if (b == RedisReply.Bulk)
            return ReadBulk();
        else if (b == RedisReply.Figure || b == RedisReply.Status)
            return ReadLine();
        else if (b == RedisReply.Error)
            return ReadLine();

        return b;
    }

    private int ReadFirstByte()
    {
        var buffer = new byte[1];
        do
        {
            socket.Receive(buffer, 0, 1, SocketFlags.None);
            if (buffer[0] != RedisReply.CR && buffer[0] != RedisReply.LF)
                break;
        } while (buffer[0] != 0);

        return buffer[0];
    }

    private object[] ReadMultiBulk()
    {
        int count = int.Parse(ReadLine());
        if (count == -1)
            return null;

        var lines = new object[count];
        for (int i = 0; i < count; i++)
        {
            lines[i] = ReadData();
        }
        return lines;
    }

    private string ReadBulk()
    {
        var size = int.Parse(ReadLine());
        if (size == -1)
            return null;

        var data = new byte[size];
        socket.Receive(data, 0, size, SocketFlags.None);
        return Encoding.UTF8.GetString(data);
    }

    private string ReadLine()
    {
        var sb = new StringBuilder();
        var buffer = new byte[1];
        do
        {
            socket.Receive(buffer, 0, 1, SocketFlags.None);
            if (buffer[0] == RedisReply.CR)
                continue;
            if (buffer[0] == RedisReply.LF)
                break;
            sb.Append((char)buffer[0]);
        } while (buffer[0] != 0);

        return sb.ToString();
    }
}