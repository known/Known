<%@ Page Title="" Language="C#" MasterPageFile="~/Admin/Admin.Master" AutoEventWireup="true" CodeBehind="UpFileList.aspx.cs" Inherits="Known.Web.Admin.UpFileList" %>
<asp:Content ID="Content1" ContentPlaceHolderID="Content" runat="server">
    <style type="text/css">
        .upfile li{float:left;margin:5px 3px 25px 0;width:90px;height:88px;border:0px solid #ccc;text-align:center;}
        .upfile .current{background:#f3f9f5;}
        .upfile p{margin:3px 0;overflow:hidden;text-overflow:ellipsis;white-space:nowrap;line-height:150%;}
        .upfile a{width:48px;height:48px;}
        .upfile img{border:0px solid #ccc;}
        .upfile .delete{position:relative;width:20px;height:20px;top:-92px;right:-40px;border:0px solid gray;color:Gray;font-size:10px;}
    </style>
    <div class="right">           
        <h4>上传附件</h4>
        <p><%=SitePath%>/upfiles/<%=DateTime.Now.ToString("yyyyMM") %></p>
        <p><input type="file" name="fuFile" id="fuFile" style="width:180px;" /></p>
        <p>存在同名文件时:</p>
        <p>
            <input id="rblistType_0" type="radio" name="rblistType" value="1" checked="checked" />
            <label for="rblistType_0">跳过</label>
            <input id="rblistType_1" type="radio" name="rblistType" value="2" />
            <label for="rblistType_1">重命名</label>
            <input id="rblistType_2" type="radio" name="rblistType" value="3" />
            <label for="rblistType_2">覆盖</label>
        </p>
        <p><input type="submit" id="btnUpload" name="btnUpload" value="上传" class="button" /></p>
        <p class="notice" style="word-wrap:break-word;word-break:break-all;overflow:auto;">允许格式:<br /><%=AllowFileExtension%>.</p>
    </div>
    <div class="left" >
        <h4>当前位置:<%=GetPathUrl()%></h4>
        <ul class="upfile">
        <%foreach(System.IO.FileSystemInfo d in CurrentDirectory.GetFileSystemInfos()){%>
            <li>
            <%if (d is System.IO.DirectoryInfo){%>
                <a href="<%=FileName%>?path=<%=CurrentPath%><%=d.Name%>/" title="点击打开此文件夹"><img src="../static/images/file/folder.png"  alt="点击打开此文件夹" width="48" height="48"/></a>
                <p class="small">
                    <span title="文件夹:<%=d.Name%>"><%=d.Name%></span>
                    <br />
                    <span class="gray"><%=((System.IO.DirectoryInfo)d).GetFileSystemInfos().Length%>个对象</span>
                </p>
            <%}else{%>
                <a href="<%=CurrentPath+d.Name%>" target="_blank" onclick="return returnValue('<%=CurrentPath+d.Name%>','<%=d.Extension%>');">
                <%if(IsImage(d.Extension)){%>
                    <img src="<%=CurrentPath+d.Name%>" width="48" height="48"/>
                <%}else{%>
                    <img src="../static/images/file/<%=GetFileImage(d.Extension)%>" width="48" height="48"/>
                <%}%>
                </a>
                <p class="small">
                    <span title="<%=d.Name%>"><%=d.Name%></span>
                    <br />
                    <span class="gray"><%=((System.IO.FileInfo)d).Length%></span>
                </p>
            <%}%>
                <a class="delete" href="<%=FileName%>?operate=delete&category=<%=d.Attributes%>&deletepath=<%=CurrentPath%><%=d.Name%>" title="删除" onclick="return confirm('确定要删除<%=d.Name%>吗?');">✖</a>
            </li>
        
        <%} if(CurrentDirectory.GetFileSystemInfos().Length == 0) { Response.Write("<p>还没有上传任何附件!</p>"); }%>
        </ul>
    </div>

    <script type="text/javascript">
        function returnValue(fileUrl, fileExtension) {
        <%if(FileName=="upfilebyeditor.aspx"){%>
            parent.addFileToEditor(fileUrl, fileExtension);
            parent.tb_remove();
            return false;
        <%}%>
        }
    </script>
</asp:Content>
