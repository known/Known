<%@ Control Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Controls.LogView" %>
<form class="form-inline">
    <div class="form-group">
        <label for="Customs">日志类型</label>
        <select id="LogType" name="LogType" class="form-control">
            <option>Bug</option>
            <option>Info</option>
        </select>
    </div>
    <span>&nbsp;</span>
    <div class="form-group">
        <label for="Function">操作时间</label>
        <input type="date" class="form-control" id="StartDate" name="StartDate">
        <span>~</span>
        <input type="date" class="form-control" id="EndDate" name="EndDate">
    </div>
    <span>&nbsp;</span>
    <button type="submit" class="btn btn-primary"><i class="glyphicon glyphicon-search"></i>&nbsp;查&nbsp;询</button>
</form>
<div class="table-responsive">
    <table class="table table-hover">
        <thead>
            <tr>
                <th>#</th>
                <th>日志类型</th>
                <th>操作人</th>
                <th>操作时间</th>
                <th>操作内容</th>
            </tr>
        </thead>
        <tbody>
        </tbody>
    </table>
</div>