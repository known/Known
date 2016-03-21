<%@ Page Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Default" %>
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width,initial-scale=1">
    <title><%=KSettings.Name%></title>
    <link href="<%=VirtualPath%>static/css/bootstrap.min.css" rel="stylesheet">
    <link href="<%=VirtualPath%>static/css/metisMenu.min.css" rel="stylesheet" />
    <link href="<%=VirtualPath%>static/css/style.css" rel="stylesheet" />
    <!-- HTML5 Shim and Respond.js IE8 support of HTML5 elements and media queries -->
    <!-- WARNING: Respond.js doesn't work if you view the page via file:// -->
    <!--[if lt IE 9]>
        <script src="https://oss.maxcdn.com/libs/html5shiv/3.7.0/html5shiv.js"></script>
        <script src="https://oss.maxcdn.com/libs/respond.js/1.4.2/respond.min.js"></script>
    <![endif]-->
</head>
<body>
    <div id="wrapper">

        <nav class="navbar navbar-default navbar-static-top navigation" role="navigation" style="margin-bottom: 0">
            <div class="navbar-header">
                <button type="button" class="navbar-toggle" data-toggle="collapse" data-target=".navbar-collapse">
                    <span class="sr-only">Toggle navigation</span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                    <span class="icon-bar"></span>
                </button>
                <div class="sys-title"><%=KSettings.Name%></div>
            </div>

            <ul class="nav navbar-top-links navbar-right">
                <li class="dropdown">
                    <a class="dropdown-toggle" data-toggle="dropdown" href="#">
                        <i class="glyphicon glyphicon-user"></i>&nbsp;&nbsp;<%=AppContext.CurrentUser.DisplayName%>&nbsp;<i class="glyphicon glyphicon-triangle-bottom"></i>
                    </a>
                    <ul class="dropdown-menu dropdown-user">
                        <li><a href="#" data-toggle="modal" data-target="#cpModal"><i class="glyphicon glyphicon-wrench"></i>&nbsp;&nbsp;修改密码</a></li>
                        <li class="divider"></li>
                        <li><a href="<%=VirtualPath%>Login.aspx"><i class="glyphicon glyphicon-off"></i>&nbsp;&nbsp;退出</a></li>
                    </ul>
                </li>
            </ul>

            <div class="navbar-default sidebar" role="navigation">
                <div class="sidebar-nav navbar-collapse">
                    <ul class="nav" id="side-menu">
                        <li>
                            <a href="<%=VirtualPath%>"><i class="glyphicon glyphicon-dashboard"></i>&nbsp;&nbsp;仪表盘</a>
                        </li>
                        <%foreach (MenuInfo menu in AppContext.CurrentUser.GetMenus(null)){%>
                        <li>
                            <a href="#"><i class="<%=menu.Icon%>"></i>&nbsp;&nbsp;<%=menu.Name%><span class="glyphicon arrow"></span></a>
                            <ul class="nav nav-second-level">
                            <%foreach (MenuInfo item in AppContext.CurrentUser.GetMenus(menu.Id)){%>
                                <li><a id="<%=item.Id%>" href="javascript:page('<%=item.Id%>');"><%=item.Name%></a></li>
                            <%}%>
                            </ul>
                        </li>
                        <%}%>
                    </ul>
                </div>
            </div>
        </nav>

        <div id="page-wrapper">
            <div class="row">
                <div class="page-title"><i class="glyphicon glyphicon-home"></i>&nbsp;<span id="breadcrumb">仪表盘</span></div>
            </div>
            <div class="row">
                <div id="content"></div>
            </div>
        </div>

    </div>

    <div class="modal fade" id="cpModal" tabindex="-1" role="dialog" aria-labelledby="cpLabel">
        <div class="modal-dialog" role="document">
            <div class="modal-content">
                <div class="modal-header">
                    <button type="button" class="close" data-dismiss="modal" aria-label="Close"><span aria-hidden="true">&times;</span></button>
                    <h4 class="modal-title" id="cpLabel">修改密码</h4>
                </div>
                <div class="modal-body">
                    <input type="hidden" id="CurrentUserId" name="CurrentUserId">
                    <div class="form-group">
                        <label for="OldPassword" class="col-sm-3 control-label">原始密码</label>
                        <div class="col-sm-9">
                            <input type="password" class="form-control" id="OldPassword" name="OldPassword" required autofocus>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="NewPassword" class="col-sm-3 control-label">新密码</label>
                        <div class="col-sm-9">
                            <input type="password" class="form-control" id="NewPassword" name="NewPassword" required>
                        </div>
                    </div>
                    <div class="form-group">
                        <label for="NewPassword1" class="col-sm-3 control-label">确认密码</label>
                        <div class="col-sm-9">
                            <input type="password" class="form-control" id="NewPassword1" name="NewPassword1" required>
                        </div>
                    </div>
                </div>
                <div class="modal-footer">
                    <button type="submit" class="btn btn-primary" onclick="return User.changePwd();"><i class="glyphicon glyphicon-ok"></i>&nbsp;保&nbsp;存</button>
                    <button type="button" class="btn btn-default" data-dismiss="modal"><i class="glyphicon glyphicon-remove"></i>&nbsp;关&nbsp;闭</button>
                </div>
            </div>
        </div>
    </div>
    
    <script src="<%=VirtualPath%>static/js/jquery.min.js"></script>
    <script src="<%=VirtualPath%>static/js/bootstrap.min.js"></script>
    <script src="<%=VirtualPath%>static/js/metisMenu.min.js"></script>
    <script type="text/javascript">var virtualPath='<%=VirtualPath%>';</script>
    <script src="<%=VirtualPath%>static/js/script.js"></script>
</body>
</html>