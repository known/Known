$(function () {

    var loginUrl = '/signin?userName={0}&password={1}';

    $('input[onenter]').keydown(function (event) {
        if (event.which === 13) {
            event.preventDefault();
            var method = $(this).attr("onenter");
            eval(method);
        }
    });

    $('#userName').focus();

    $('#btnLogin').click(function () {
        var userName = $('#userName').val();
        var password = $('#password').val();
        if (!userName) {
            _showMessage('请输入用户名！');
            return;
        }
        if (!password) {
            _showMessage('请输入密码！');
            return;
        }

        _showMessage('');
        var $this = $(this).attr('disabled', 'disabled').val('登录中...');
        $.post(loginUrl.format(userName, $.md5(password)), function (result) {
            _showMessage(result.message);
            if (result.ok) {
                User.setUser(result.data);
                location = '/';
            } else {
                $this.removeAttr('disabled').val('登录');
            }
        });

        function _showMessage(message) {
            $('#error').html(message);
        }
    });

});