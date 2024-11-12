namespace Known.Admin.Extensions;

static class ModelExtension
{
    #region MenuInfo
    internal static List<CodeInfo> GetAllActions(this MenuInfo info)
    {
        var codes = new List<CodeInfo>();
        if (info.Tools != null && info.Tools.Count > 0)
            codes.AddRange(info.Tools.Select(b => GetAction(info, b)));
        if (info.Actions != null && info.Actions.Count > 0)
            codes.AddRange(info.Actions.Select(b => GetAction(info, b)));
        return codes;
    }

    internal static List<CodeInfo> GetAllColumns(this MenuInfo info)
    {
        var codes = new List<CodeInfo>();
        if (info.Columns != null && info.Columns.Count > 0)
            codes.AddRange(info.Columns.Select(b => new CodeInfo($"c_{info.Id}_{b.Id}", b.Name)));
        return codes;
    }

    private static CodeInfo GetAction(MenuInfo menu, string id)
    {
        var code = $"b_{menu.Id}_{id}";
        var button = Config.Actions.FirstOrDefault(b => b.Id == id);
        var name = button != null ? button.Name : id;
        return new CodeInfo(code, name);
    }
    #endregion

    #region Organization
    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models)
    {
        MenuInfo current = null;
        return models.ToMenuItems(ref current);
    }

    internal static List<MenuInfo> ToMenuItems(this List<SysOrganization> models, ref MenuInfo current)
    {
        var menus = new List<MenuInfo>();
        if (models == null || models.Count == 0)
            return menus;

        var tops = models.Where(m => m.ParentId == "0").OrderBy(m => m.Code).ToList();
        foreach (var item in tops)
        {
            var menu = CreateMenuInfo(item);
            if (current != null && current.Id == menu.Id)
                current = menu;

            menus.Add(menu);
            AddChildren(models, menu, ref current);
        }

        current ??= menus[0];
        return menus;
    }

    private static void AddChildren(List<SysOrganization> models, MenuInfo menu, ref MenuInfo current)
    {
        var items = models.Where(m => m.ParentId == menu.Id).OrderBy(m => m.Code).ToList();
        if (items == null || items.Count == 0)
            return;

        foreach (var item in items)
        {
            item.ParentName = menu.Name;
            var sub = CreateMenuInfo(item);
            sub.Parent = menu;
            if (current != null && current.Id == sub.Id)
                current = sub;

            menu.Children.Add(sub);
            AddChildren(models, sub, ref current);
        }
    }

    private static MenuInfo CreateMenuInfo(SysOrganization model)
    {
        return new MenuInfo
        {
            Id = model.Id,
            ParentId = model.ParentId,
            Code = model.Code,
            Name = model.Name,
            Data = model
        };
    }
    #endregion

    #region User
    /// <summary>
    /// 发送站内消息。
    /// </summary>
    /// <param name="user">用户对象。</param>
    /// <param name="db">数据库对象。</param>
    /// <param name="toUser">站内收件人。</param>
    /// <param name="subject">消息主题。</param>
    /// <param name="content">消息内容。</param>
    /// <param name="isUrgent">是否紧急。</param>
    /// <param name="filePath">附件路径。</param>
    /// <param name="bizId">关联业务数据ID。</param>
    /// <returns></returns>
    public static Task SendMessageAsync(this UserInfo user, Database db, string toUser, string subject, string content, bool isUrgent = false, string filePath = null, string bizId = null)
    {
        var level = isUrgent ? Constant.UMLUrgent : Constant.UMLGeneral;
        return SendMessageAsync(db, user, level, toUser, subject, content, filePath, bizId);
    }

    private static Task SendMessageAsync(Database db, UserInfo user, string level, string toUser, string subject, string content, string filePath = null, string bizId = null)
    {
        var model = new SysMessage
        {
            UserId = toUser,
            Type = Constant.UMTypeReceive,
            MsgBy = user.Name,
            MsgLevel = level,
            Subject = subject,
            Content = content,
            FilePath = filePath,
            IsHtml = true,
            Status = Constant.UMStatusUnread,
            BizId = bizId
        };
        return db.SaveAsync(model);
    }
    #endregion'
}