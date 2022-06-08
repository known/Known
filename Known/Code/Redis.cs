/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Text;

namespace Known
{
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

    public class RedisClient : IDisposable
    {
        private readonly string host;
        private readonly int port;
        private Socket socket;
        private readonly byte[] buffer = new byte[100000];

        public RedisClient(string host = "localhost", int? port = 6379)
        {
            this.host = host;
            this.port = port.Value;
        }

        public string Password { get; set; }

        public virtual void Dispose()
        {
            Close();
        }

        public bool Ping()
        {
            return ExecuteCommand(RedisCmd.PING).Equals(RedisReply.Pong);
        }

        public bool Select()
        {
            return ExecuteCommand(RedisCmd.SELECT).Equals(RedisReply.Success);
        }

        public string GetServerInfo()
        {
            return ExecuteCommand(RedisCmd.INFO).ToString();
        }

        public T Get<T>(string key)
        {
            var result = ExecuteCommand(RedisCmd.GET, key);
            if (result == null)
                return default;

            var json = result.ToString();
            return Utils.FromJson<T>(json);
        }

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

        public int Delete(string key)
        {
            var reply = ExecuteCommand(RedisCmd.DEL, key).ToString();
            if (int.TryParse(reply, out int nums))
                return nums;

            throw new Exception(reply);
        }

        private void Connect()
        {
            if (socket == null)
                socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

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
            {
                return ReadMultiBulk();
            }
            else if (b == RedisReply.Bulk)
            {
                return ReadBulk();
            }
            else if (b == RedisReply.Figure || b == RedisReply.Status)
            {
                return ReadLine();
            }
            else if (b == RedisReply.Error)
            {
                return ReadLine();
            }

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
}
