﻿namespace Known.WorkFlows;

/// <summary>
/// 系统工作流基类。
/// </summary>
/// <param name="context">上下文对象。</param>
public abstract class FlowBase(Context context)
{
    /// <summary>
    /// 取得上下文对象。
    /// </summary>
    public Context Context { get; } = context;

    /// <summary>
    /// 流程表单提交前，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns>操作结果。</returns>
    public virtual Task<Result> OnCommitingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 流程表单提交后，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns></returns>
    public virtual Task OnCommitedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;

    /// <summary>
    /// 流程表单撤回前，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns>操作结果。</returns>
    public virtual Task<Result> OnRevokingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 流程表单撤回后，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns></returns>
    public virtual Task OnRevokedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;

    /// <summary>
    /// 流程表单审核前，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns>操作结果。</returns>
    public virtual Task<Result> OnVerifingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 流程表单审核后，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns></returns>
    public virtual Task OnVerifiedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;

    /// <summary>
    /// 流程表单重启前，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns>操作结果。</returns>
    public virtual Task<Result> OnRepeatingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 流程表单重启后，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns></returns>
    public virtual Task OnRepeatedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;

    /// <summary>
    /// 流程表单停止前，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns>操作结果。</returns>
    public virtual Task<Result> OnStoppingAsync(Database db, FlowFormInfo info) => Result.SuccessAsync("");

    /// <summary>
    /// 流程表单停止后，异步调用虚方法。
    /// </summary>
    /// <param name="db">数据库访问对象。</param>
    /// <param name="info">流程表单信息。</param>
    /// <returns></returns>
    public virtual Task OnStoppedAsync(Database db, FlowFormInfo info) => Task.CompletedTask;
}