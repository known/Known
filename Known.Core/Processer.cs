namespace Known.Core;

/// <summary>
/// 进程启动结果类型。
/// </summary>
public class StartResult
{
    /// <summary>
    /// 取得或设置进程ID。
    /// </summary>
    public int Id { get; set; }

    /// <summary>
    /// 取得或设置是否启动成功。
    /// </summary>
    public bool IsSuccess { get; set; }

    /// <summary>
    /// 取得或设置进程名称。
    /// </summary>
    public string ProcessName { get; set; }

    /// <summary>
    /// 取得或设置进程文件版本。
    /// </summary>
    public string FileVersion { get; set; }
}

/// <summary>
/// 进程操作类。
/// </summary>
public sealed class Processer
{
    private Processer() { }

    /// <summary>
    /// 启动一个可执行的exe文件进程。
    /// </summary>
    /// <param name="exeFile">exe文件路径。</param>
    /// <param name="arg">启动参数。</param>
    /// <returns>启动结果。</returns>
    public static StartResult Start(string exeFile, string arg = null)
    {
        var process = new Process();
        process.StartInfo.FileName = exeFile;
        if (!string.IsNullOrEmpty(arg))
        {
            process.StartInfo.Arguments = arg;
        }
        process.StartInfo.UseShellExecute = true;
        process.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
        process.Start();

        bool isSuccess;
        while (true)
        {
            isSuccess = Exists(process.ProcessName, exeFile);
            if (isSuccess)
                break;
            Thread.Sleep(500);
        }

        return new StartResult
        {
            Id = process.Id,
            IsSuccess = isSuccess,
            ProcessName = process.ProcessName,
            FileVersion = process.MainModule?.FileVersionInfo.FileVersion
        };
    }

    /// <summary>
    /// 停止一个进程。
    /// </summary>
    /// <param name="processName">进程名称。</param>
    /// <param name="fileName">进程文件路径。</param>
    public static void Stop(string processName, string fileName)
    {
        if (string.IsNullOrEmpty(processName))
            return;

        var lists = Process.GetProcessesByName(processName);
        foreach (var item in lists)
        {
            if (item.MainModule?.FileName == fileName)
            {
                item.Kill();
            }
        }
    }

    /// <summary>
    /// 判断进程是否存在。
    /// </summary>
    /// <param name="processName">进程名称。</param>
    /// <param name="fileName">进程文件路径。</param>
    /// <returns>存在返回True，否则返回False。</returns>
    public static bool Exists(string processName, string fileName)
    {
        if (string.IsNullOrEmpty(processName))
            return false;

        var lists = Process.GetProcessesByName(processName);
        foreach (var item in lists)
        {
            if (item.MainModule?.FileName == fileName)
            {
                return true;
            }
        }

        return false;
    }
}