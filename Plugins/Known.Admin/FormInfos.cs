﻿namespace Known;

/// <summary>
/// 修改密码表单信息类。
/// </summary>
public class PwdFormInfo
{
    /// <summary>
    /// 取得或设置原始密码。
    /// </summary>
    [Form(Type = "Password"), Required]
    public string OldPwd { get; set; }

    /// <summary>
    /// 取得或设置新密码。
    /// </summary>
    [Form(Type = "Password"), Required]
    public string NewPwd { get; set; }

    /// <summary>
    /// 取得或设置确认新密码。
    /// </summary>
    [Form(Type = "Password"), Required]
    public string NewPwd1 { get; set; }
}

/// <summary>
/// 用户头像信息类。
/// </summary>
public class AvatarInfo
{
    /// <summary>
    /// 取得或设置用户ID。
    /// </summary>
    public string UserId { get; set; }

    /// <summary>
    /// 取得或设置用户头像文件信息。
    /// </summary>
    public FileDataInfo File { get; set; }
}

/// <summary>
/// 数据导入表单信息类。
/// </summary>
public class ImportFormInfo : FileFormInfo
{
    /// <summary>
    /// 取得或设置导入名称。
    /// </summary>
    public string Name { get; set; }

    /// <summary>
    /// 取得或设置是否是异步导入。
    /// </summary>
    public bool IsAsync { get; set; }

    /// <summary>
    /// 取得或设置导入是否已完成，默认True。
    /// </summary>
    public bool IsFinished { get; set; } = true;

    /// <summary>
    /// 取得或设置异步导入反馈的提示信息。
    /// </summary>
    public string Message { get; set; }

    /// <summary>
    /// 取得或设置导入校验的错误信息。
    /// </summary>
    public string Error { get; set; }

    /// <summary>
    /// 根据模型类型获取导入栏位名称列表，适用于自动生成导入规范（暂未使用）。
    /// </summary>
    /// <param name="modelType">模型类型。</param>
    /// <returns>导入栏位名称列表。</returns>
    public static List<string> GetImportColumns(string modelType)
    {
        var columns = new List<string>();
        var baseProperties = TypeHelper.Properties(typeof(EntityBase));
        var type = Type.GetType(modelType);
        var properties = TypeHelper.Properties(type);
        foreach (var item in properties)
        {
            if (item.GetGetMethod().IsVirtual || baseProperties.Any(p => p.Name == item.Name))
                continue;

            var name = item.DisplayName();
            if (!string.IsNullOrWhiteSpace(name))
                columns.Add(name);
        }
        return columns;
    }
}