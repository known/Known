using System.Net;
using System.Net.Mail;

namespace Known;

/// <summary>
/// 电子邮件配置信息类。
/// </summary>
public class EmailConfigInfo
{
    /// <summary>
    /// 取得或设置SMTP服务器地址。
    /// </summary>
    public string SmtpServer { get; set; }

    /// <summary>
    /// 取得或设置SMTP服务器端口。
    /// </summary>
    public int SmtpPort { get; set; }

    /// <summary>
    /// 取得或设置是否启用SSL。
    /// </summary>
    public bool EnableSsl { get; set; }

    /// <summary>
    /// 取得或设置用户名。
    /// </summary>
    public string UserName { get; set; }

    /// <summary>
    /// 取得或设置密码。
    /// </summary>
    public string Password { get; set; }

    /// <summary>
    /// 取得或设置发件人显示名称。
    /// </summary>
    public string FromName { get; set; }

    /// <summary>
    /// 取得或设置发件人邮箱地址。
    /// </summary>
    public string FromMail { get; set; }
}

/// <summary>
/// 电子邮件信息类。
/// </summary>
public class EmailInfo
{
    /// <summary>
    /// 取得或设置收件人邮箱地址。
    /// </summary>
    public List<EmailAddressInfo> ToMails { get; set; }

    /// <summary>
    /// 取得或设置抄送收件人邮箱地址。
    /// </summary>
    public List<EmailAddressInfo> CCMails { get; set; }

    /// <summary>
    /// 取得或设置密送收件人邮箱地址。
    /// </summary>
    public List<EmailAddressInfo> BccMails { get; set; }

    /// <summary>
    /// 取得或设置邮件主题。
    /// </summary>
    public string Subject { get; set; }

    /// <summary>
    /// 取得或设置邮件内容。
    /// </summary>
    public string Body { get; set; }

    /// <summary>
    /// 取得或设置邮件内容是否是HTML格式。
    /// </summary>
    public bool IsBodyHtml { get; set; }

    /// <summary>
    /// 取得或设置附件路径。
    /// </summary>
    public List<string> Attachments { get; set; }
}

/// <summary>
/// 邮件地址信息类。
/// </summary>
public class EmailAddressInfo
{
    /// <summary>
    /// 取得或设置显示名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置邮件地址。
    /// </summary>
    public string Address { get; set; }
}

[Task(BizType)]
class EmailTask : TaskBase
{
    public const string BizType = "Email";

    internal static SysTask CreateTask(EmailInfo info)
    {
        return new SysTask
        {
            BizId = string.Join(",", info.ToMails),
            Type = BizType,
            Name = info.Subject,
            Target = Utils.ToJson(info),
            Status = TaskJobStatus.Pending
        };
    }

    public override async Task<Result> ExecuteAsync(Database db, SysTask task)
    {
        var config = Config.App.Email;
        if (config == null || string.IsNullOrWhiteSpace(config.SmtpServer))
            return Result.Error("邮件服务未配置！");

        var info = Utils.FromJson<EmailInfo>(task.Target);
        if (info == null)
            return Result.Error("邮件信息不能为空！");

        if (info.ToMails == null || info.ToMails.Count == 0)
            return Result.Error("收件人不能为空！");

        try
        {
            if (config.SmtpPort == 25)
            {
                ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls12;
                //ServicePointManager.ServerCertificateValidationCallback = (s, cert, chain, errors) => true;
            }
            using (var client = new SmtpClient(config.SmtpServer, config.SmtpPort))
            {
                if (config.SmtpPort != 80)
                    client.EnableSsl = config.EnableSsl;
                client.UseDefaultCredentials = false;
                client.Credentials = new NetworkCredential(config.UserName, config.Password);
                client.DeliveryMethod = SmtpDeliveryMethod.Network;
                client.Timeout = 15000;
                client.SendCompleted += (sender, e) =>
                {
                    if (e.Error != null)
                        Console.WriteLine($"发送错误: {e.Error.Message}");
                };
                var message = GetMailMessage(config, info);
                client.Send(message);
            }
            return Result.Success("邮件发送成功！");
        }
        catch (Exception ex)
        {
            Logger.Exception(LogTarget.Task, db.User, ex);
            return Result.Error($"邮件发送失败：{ex.Message}");
        }
    }

    private static MailMessage GetMailMessage(EmailConfigInfo config, EmailInfo info)
    {
        var message = new MailMessage
        {
            From = new MailAddress(config.FromMail, config.FromName),
            Subject = info.Subject,
            Body = info.Body,
            IsBodyHtml = info.IsBodyHtml
        };
        AddEmails(message.To, info.ToMails);
        AddEmails(message.CC, info.CCMails);
        AddEmails(message.Bcc, info.BccMails);

        if (info.Attachments != null && info.Attachments.Count > 0)
        {
            foreach (var item in info.Attachments)
            {
                message.Attachments.Add(new Attachment(item));
            }
        }

        return message;
    }

    private static void AddEmails(MailAddressCollection mails, List<EmailAddressInfo> infos)
    {
        if (infos == null || infos.Count == 0)
            return;

        foreach (var item in infos)
        {
            mails.Add(new MailAddress(item.Address, item.Name));
        }
    }
}