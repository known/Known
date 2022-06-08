var copyrightInfo = '&copy;2020-' + new Date().format('yyyy') + '<a href="http://www.pumantech.com" target="_blank">普漫科技</a>';

function Login() {
    //fields

    //methods
    this.render = function () {
        $('.topbar').hide();
        $('#app .router').css({ top: 0 });
        var elem = $('<div>').addClass('login');
        var form = $('<div>').addClass('form').appendTo(elem);
        _createHeader(form);
        _createBody(form);
        _createFooter(elem);
        return elem;
    }

    this.mounted = function () {
        $('#userName').focus();
    }

    //private
    function _createHeader(dom) {
        $('<h1>').html($('title').text()).appendTo(dom);
    }

    function _createBody(dom) {
        _createFormItem(dom, 'userName', 'text', 'fa fa-user', Language.UserName, '$("#password").focus();');
        var enter = showCaptcha ? '$("#captcha").focus();' : '$("#btnLogin").click();';
        _createFormItem(dom, 'password', 'password', 'fa fa-lock', Language.Password, enter);

        if (showCaptcha) {
            _createCaptcha(dom);
        }

        $('<button>').attr('id', 'btnLogin')
            .html(Language.Login)
            .appendTo(dom)
            .on('click', function () { _login($(this)); });
        $('<div>').attr('id', 'message').addClass('message').appendTo(dom);
    }

    function _createFormItem(form, id, type, icon, placeholder, enter) {
        var item = $('<div>').addClass('form-item').appendTo(form);
        $('<label>').addClass(icon).attr('for', id).appendTo(item);
        $('<input>')
            .attr('type', type)
            .attr('id', id)
            .attr('placeholder', placeholder)
            .attr('autocomplete', 'off')
            .attr('onenter', enter)
            .appendTo(item);
        return item;
    }

    function _createCaptcha(form) {
        var item = _createFormItem(form, 'captcha', 'text', 'fa fa-check-circle-o', Language.Captcha, '$("#btnLogin").click();');
        $('<img>')
            .attr('id', 'imgCaptcha')
            .attr('src', baseUrl + '/Home/GetCaptcha')
            .attr('title', Language.RefreshCaptcha)
            .attr('alt', '')
            .appendTo(item)
            .on('click', function () {
                var img = $(this);
                var src = img.attr('src').split('?')[0];
                img.attr('src', src + '?rnd=' + Math.random());
            });
    }

    function _createFooter(dom) {
        var footer = $('<div>').addClass('footer').appendTo(dom);
        //$('<p class="support">')
        //    .append(Language.TechSupport + '：')
        //    .append('<a href="' + config.app.SupportUrl + '" target="_blank">' + config.app.SupportName + '</a>')
        //    .appendTo(footer);
        $('<p>').addClass('copyright')
            .append(copyrightInfo)
            .appendTo(footer);
    }

    function _login(obj) {
        $('.form-item').removeClass('error');
        var userName = _getFormValue('userName');
        var password = _getFormValue('password');
        var captcha = _getFormValue('captcha');
        if (userName === '' || password === '' || (showCaptcha && captcha === ''))
            return;

        var text = obj.html();
        var btn = obj.attr('disabled', true).html(Language.Logining);
        $.post(baseUrl + '/signin', {
            userName: userName, password: password,
            captcha: captcha, isMobile: true
        }, function (result) {
            btn.removeAttr('disabled').html(text);
            if (result.IsValid) {
                _setMessage('#fff', result.Message);
                curUser = result.Data.User;
                Utils.setUser(curUser);
                location.reload();
            } else {
                _setMessage('#f00', result.Message);
            }
        });
    }

    function _getFormValue(id) {
        var elem = $('#' + id);
        var value = $.trim(elem.val());
        if (value === '') {
            elem.parent().addClass('error');
        }
        return value;
    }

    function _setMessage(color, message) {
        $('#message').css('color', color).html(message);
    }
}