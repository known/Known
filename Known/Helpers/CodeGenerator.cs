namespace Known.Helpers;

/// <summary>
/// 代码生成器接口。
/// </summary>
public interface ICodeGenerator
{
    /// <summary>
    /// 获取数据库建表脚本。
    /// </summary>
    /// <param name="dbType">数据库类型。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>建表脚本。</returns>
    string GetScript(DatabaseType dbType, EntityInfo entity);

    /// <summary>
    /// 获取模型信息类代码。
    /// </summary>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>实体类代码。</returns>
    string GetModel(EntityInfo entity);

    /// <summary>
    /// 获取实体类代码。
    /// </summary>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>实体类代码。</returns>
    string GetEntity(EntityInfo entity);

    /// <summary>
    /// 获取页面组件代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>页面组件代码。</returns>
    string GetPage(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取表单组件代码。
    /// </summary>
    /// <param name="page">表单模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>表单组件代码。</returns>
    string GetForm(FormInfo page, EntityInfo entity);

    /// <summary>
    /// 获取业务服务接口代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <param name="hasClient">是否含有客户端类。</param>
    /// <returns>业务服务接口代码。</returns>
    string GetIService(PageInfo page, EntityInfo entity, bool hasClient = false);

    /// <summary>
    /// 获取客户端HTTP请求代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>客户端HTTP请求代码。</returns>
    string GetClient(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取业务逻辑服务层代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>业务逻辑服务层代码。</returns>
    string GetService(PageInfo page, EntityInfo entity);

    /// <summary>
    /// 获取数据依赖访问层代码。
    /// </summary>
    /// <param name="page">页面模型对象。</param>
    /// <param name="entity">实体模型对象。</param>
    /// <returns>数据依赖访问层代码。</returns>
    string GetRepository(PageInfo page, EntityInfo entity);
}

[Service(ServiceLifetime.Singleton)]
partial class CodeGenerator : ICodeGenerator
{
    public string GetRepository(PageInfo page, EntityInfo entity)
    {
        var sb = new StringBuilder();
        sb.AppendLine("namespace {0}.Repositories;", Config.App.Id);
        sb.AppendLine(" ");
        sb.AppendLine("class XXXRepository");
        sb.AppendLine("{");
        sb.AppendLine("    //{0}", entity.Id);
        sb.AppendLine("    internal static Task<PagingResult<{0}>> Query{0}sAsync(Database db, PagingCriteria criteria)", entity.Id);
        sb.AppendLine("    {");
        sb.AppendLine("        var sql = \"select * from {0} where CompNo=@CompNo\";", entity.Id);
        sb.AppendLine("        return db.QueryPageAsync<{0}>(sql, criteria);", entity.Id);
        sb.AppendLine("    }");
        sb.AppendLine("}");
        return sb.ToString();
    }
}