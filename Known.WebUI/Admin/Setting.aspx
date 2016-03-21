<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="Setting.aspx.cs" Inherits="Known.Web.Admin.Setting" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <style type="text/css">
    .navs{line-height:22px; border:0px solid #ccc;color:#666; padding:3px  ;}
    .navs a { padding-right:8px; font-size:14px;}
    </style>
    <div class="navs">
        <a href="#bases">基本设置</a> <a href="#footers">页脚设置</a>
    </div>
    <h4 id="bases">基本设置</h4>
    <table>
        <tr class="row">
            <td style="width:20%;"><label for="SiteName">网站名称</label></td>
            <td>
                <input type="text" id="SiteName" name="SiteName" placeholder="这里填您的网站标题" value="<%=setting.SiteName%>" class="text" style="width:70%;" />
            </td>
        </tr>
        <tr class="row">
            <td><label for="SiteDescription">网站描述</label></td>
            <td>
                <input type="text" id="SiteDescription" name="SiteDescription" placeholder="这里填您的网站描述" value="<%=setting.SiteDescription%>" class="text" style="width:70%;" />
            </td>
        </tr>
        <tr class="row">
            <td><label for="MetaKeywords">Meta Keywords</label></td>
            <td>
                <input type="text" id="MetaKeywords" name="MetaKeywords" placeholder="这里填您的网站关键字,用逗号隔开" value="<%=setting.MetaKeywords%>" class="text" style="width:70%;" />
                <span class="m_desc">首页关键字,用逗号","隔开</span>
            </td>
        </tr>
        <tr class="row">
            <td><label for="MetaDescription">Meta Description</label></td>
            <td>
                <input type="text" id="MetaDescription" name="MetaDescription" placeholder="这里填写您网站的简介" value="<%=setting.MetaDescription%>" class="text" style="width:70%;" />
                <span class="m_desc">首页描述</span>
            </td>
        </tr>
        <tr class="row">
            <td></td>
            <td>
                <input type="checkbox" id="Enabled" name="Enabled"<%if(setting.Enabled){%> checked="checked"<%}%> />
                <label for="Enabled">启用网站</label>
            </td>
        </tr>
    </table>

    <h4 id="footers">页脚设置<span class="small gray normal">(支持Html,网站统计,备案号等可放于此)</span></h4>
    <table>
        <tr class="row">
            <td style="width:20%;"><label for="FooterHtml">页脚内容</label></td>
            <td>
                <textarea id="FooterHtml" name="FooterHtml" rows="2" cols="20" style="height:100px;width:80%;"><%=setting.FooterHtml%></textarea>
            </td>
        </tr>
        <tr class="rowend">
            <td>&nbsp;</td>
            <td><input type="submit" value="保存" class="button" /></td>
        </tr>
    </table>

</asp:Content>
