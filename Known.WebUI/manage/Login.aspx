<%@ Page Language="C#" AutoEventWireup="true" Inherits="Known.Web.Admin.Login" %>
<!DOCTYPE html>
<html>
<head>
    <meta name="viewport" content="width=device-width,initial-scale=1">
    <title><%=KSettings.Name%></title>
    <link href="static/css/bootstrap.min.css" rel="stylesheet">
    <link href="static/css/login.css" rel="stylesheet" />
</head>
<body>
    <div class="container">
        <form class="form-signin" method="post">
            <div class="alert-danger" role="alert"><%=AppContext.AlertMessage%></div>
            <h2 class="heading"><%=KSettings.Name%></h2>
            <input type="text" id="userName" name="userName" class="form-control" placeholder="用户名" required autofocus>
            <input type="password" id="password" name="password" class="form-control" placeholder="密码" required>
            <div class="checkbox">
                <label><input type="checkbox" value="remember-me"> 记住我 </label>
            </div>
            <button type="submit" class="btn btn-lg btn-primary btn-block">登 录</button>
        </form>
    </div>
</body>
</html>
