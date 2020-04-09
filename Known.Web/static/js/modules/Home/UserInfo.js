layui.define('index', function (exports) {
    var url = {
        GetUserInfo: '/Home/GetUserInfo',
        SaveUserInfo: '/Home/SaveUserInfo',
        UpdatePassword: '/Home/UpdatePassword'
    };

    var $ = layui.jquery,
        form = layui.form,
        layer = layui.layer;

    $.get(url.GetUserInfo, function (result) {
        form.val('formUserInfo', result);
    });

    form.on('submit(saveUserInfo)', function (data) {
        $.post(url.SaveUserInfo, {
            data: JSON.stringify(data.field)
        }, function (result) {
            layer.msg(result.message);
        });
        return false;
    });

    form.verify({
        pass: [/^[\S]{6,12}$/, '密码必须6到12位，且不能出现空格！'],
        repass: function (t) {
            if (t !== $('#password').val())
                return '两次密码输入不一致！'
        }
    });

    form.on('submit(savePassword)', function (data) {
        $.post(url.UpdatePassword, data.field, function (result) {
            layer.msg(result.message);
        });
        return false;
    });

    exports('/Home/UserInfo', {});
});