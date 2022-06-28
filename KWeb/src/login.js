/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-26     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

function Login() {
    //fields
    var isMobile = Utils.checkMobile();
    var appName = 'Known快速开发平台';
    var lang = {
        UserLogin: '用户登录',
        ScanCode: '手机扫码',
        UserName: '用户名',
        Password: '密码',
        Captcha: '验证码',
        RefreshCaptcha: '单击可刷新',
        Login: '登 录',
        Logining: '登录中...',
    };

    //methods
    this.render = function (dom) {
        $('body').css('background-image', 'url(/img/bg.jpg)');
        
        _createHeader(dom);
        _createBody(dom);
        _createFooter(dom);
    }

    this.mounted = function () {
        $('#userName').focus();
    }

    //private
    function _createHeader(dom) {
        var header = $('<div>').addClass('login-header').appendTo(dom);
        $('<h2>').html(appName).appendTo(header);
    }

    function _createBody(dom) {
        var login = $('<div>').addClass('login').attr('id', 'login').appendTo(dom);
        var body = $('<div>').addClass('login-body').appendTo(login);

        if (!isMobile) {
            _createBell(body);
        }

        _createLogin1(body);

        if (!isMobile) {
            _createLogin2(body);
        }
    }

    function _createBell(body) {
        var bell = $('<div>').addClass('login-bell').appendTo(body);
        $('<img>')
            .attr('src', '/img/login_icon1.png')
            .attr('alt', 'login')
            .appendTo(bell)
            .on('click', function () {
                var icon = $(this).data('icon') || '1';
                icon = icon === '1' ? '2' : '1';
                var src = '/img/login_icon' + icon + '.png';
                $(this).data('icon', icon).attr('src', src);
                $('.login-form').hide();
                $('#login' + icon).show();
            });
    }

    function _createLogin1(body) {
        var form = $('<div>').addClass('login-form').attr('id', 'login1').appendTo(body);
        var title = $('<div>').addClass('login-title').attr('id', 'ltLogin').append(lang.UserLogin).appendTo(form);
        title.css('padding-bottom', '0');

        _createFormItem(form, 'userName', 'text', 'fa fa-user', lang.UserName, '$("#password").focus()');
        _createFormItem(form, 'password', 'password', 'fa fa-lock', lang.Password, '$("#captcha").focus()');
        _createCaptcha(form);

        var item = $('<div>').addClass('login-form-item').appendTo(form);
        $('<button>').addClass('btn-primary')
            .attr('id', 'btnLogin')
            .html(lang.Login)
            .appendTo(item)
            .on('click', function () {
                _login($(this), false);
            });

        $('<div>').attr('id', 'message').appendTo(form);
    }

    function _createFormItem(form, id, type, icon, placeholder, onenter) {
        var item = $('<div>').addClass('login-form-item').appendTo(form);
        $('<label>').addClass(icon).attr('for', id).appendTo(item);
        $('<input>')
            .attr('type', type)
            .attr('id', id)
            .attr('placeholder', placeholder)
            .attr('autocomplete', 'off')
            .attr('onenter', onenter)
            .appendTo(item);
        return item;
    }

    function _createCaptcha(form) {
        var item = _createFormItem(form, 'captcha', 'text', 'fa fa-check-circle-o', lang.Captcha, '$("#btnLogin").click()');
        $('<img>')
            .attr('id', 'imgCaptcha')
            .attr('src', '/Home/GetCaptcha')
            .attr('title', lang.RefreshCaptcha)
            .attr('alt', '')
            .appendTo(item)
            .on('click', function () {
                var img = $(this);
                var src = img.attr('src').split('?')[0];
                img.attr('src', src + '?rnd=' + Math.random());
            });
    }

    function _createLogin2(body) {
        var form = $('<div>').addClass('login-form').attr('id', 'login2').appendTo(body);
        form.hide();
        $('<div>')
            .addClass('login-title')
            .attr('id', 'ltScan')
            .append(lang.ScanCode)
            .appendTo(form);
        var qrcode = $('<div>').addClass('login-qrcode').appendTo(form);
        $('<img>')
            .attr('src', '/img/qrcode.png')
            .attr('alt', 'qrcode')
            .appendTo(qrcode);
    }

    function _createFooter(dom) {
        var footer = $('<div>').addClass('login-footer').appendTo(dom);
        //$('<p class="support">')
        //    .append(Language.TechSupport + '：')
        //    .append('<a href="' + config.app.SupportUrl + '" target="_blank">' + config.app.SupportName + '</a>')
        //    .appendTo(footer);
        $('<p>').addClass('copyright')
            .append('')
            .appendTo(footer);
    }

    function _login(obj, force) {
        $('.login-form-item input').removeClass('error');
        var userName = _getFormValue('userName');
        var password = _getFormValue('password');
        var captcha = _getFormValue('captcha');
        var cid = Utils.getClientId();
        if (userName === '' || password === '' || captcha === '')
            return;

        var text = obj.html();
        var btn = obj.attr('disabled', true).html(lang.Logining);
        $.post('/signin', {
            userName: userName, password: password,
            captcha: captcha, cid: cid, force: force
        }, function (res) {
            btn.removeAttr('disabled').html(text);
            if (res.IsValid) {
                _setMessage('#00f', res.Message);
                location = location;
            } else {
                if (res.Data && res.Data.Confirm) {
                    Layer.confirm(res.Message, function () {
                        _login(obj, true);
                    });
                } else {
                    _setMessage('#f00', res.Message);
                }
            }
        });
    }

    function _getFormValue(id) {
        var elem = $('#' + id);
        var value = $.trim(elem.val());
        if (value === '') {
            elem.addClass('error');
        }
        return value;
    }

    function _setMessage(color, message) {
        $('#message').css('color', color).html(message);
    }

    //var stars = 800;  /*星星的密集程度，数字越大越多*/
    //var $stars = $('.stars');
    //var r = 800;   /*星星的看起来的距离,值越大越远,可自行调制到自己满意的样子*/
    //for (var i = 0; i < stars; i++) {
    //    var $star = $('<div/>').addClass('star');
    //    $stars.append($star);
    //}
    //$('.star').each(function () {
    //    var cur = $(this);
    //    var s = 0.2 + (Math.random() * 1);
    //    var curR = r + (Math.random() * 300);
    //    cur.css({
    //        transformOrigin: '0 0 ' + curR + 'px',
    //        transform: ' translate3d(0,0,-' + curR + 'px) rotateY(' + (Math.random() * 360) + 'deg) rotateX(' + (Math.random() * -50) + 'deg) scale(' + s + ',' + s + ')'
    //    });
    //});
}

app.route({ component: new Login() });