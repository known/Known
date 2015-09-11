<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="PostEdit.aspx.cs" Inherits="Known.Web.Admin.PostEdit" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <script src="../static/vendors/ckeditor/ckeditor.js"></script>
    <p>
        <label>标题</label>
        <input type="text" id="txtTitle" name="txtTitle" style="width:95%" />
    </p>
    <p>
        <label>内容</label>
        <textarea id="txtContent" name="txtContent"></textarea>
        <label class="label gray">提示：Enter产生&lt;p&gt;(换段), Shift+Enter产生&lt;br/&gt;(换行)</label>
    </p>
    <script type="text/javascript">
        function addFileToEditor(fileUrl, fileExtension) {
            if (fileExtension == '.gif' || fileExtension == '.jpg' || fileExtension == '.jpeg' || fileExtension == '.bmp' || fileExtension == '.png') {
                var imageTag = "<img src=\"" + fileUrl + "\"/>";
                CKEDITOR.instances.txtContent.insertHtml(imageTag);
            } else {
                var imageTag = "<a href=\"" + fileUrl + "\">" + fileUrl + "</a>";
                CKEDITOR.instances.txtContent.insertHtml(imageTag);
            }
        }

        CKEDITOR.replace('txtContent', { height: 320 });
    </script>
</asp:Content>
