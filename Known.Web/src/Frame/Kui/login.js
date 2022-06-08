/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

var copyrightInfo = '&copy;2020-' + new Date().format('yyyy') + '<a href="http://www.pumantech.com" target="_blank">普漫科技</a>';

function Login() {
    //fields
    var isMobile = Utils.checkMobile();

    //methods
    this.render = function (dom) {
        $('body').css('background-image', 'url(' + staticUrl + '/img/bg.jpg)');
        
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

        if (!isMobile && hasMobile) {
            _createBell(body);
        }

        _createLogin1(body);

        if (!isMobile && hasMobile) {
            _createLogin2(body);
        }
    }

    function _createBell(body) {
        var bell = $('<div>').addClass('login-bell').appendTo(body);
        $('<img>')
            .attr('src', staticUrl + '/img/login_icon1.png')
            .attr('alt', 'login')
            .appendTo(bell)
            .on('click', function () {
                var icon = $(this).data('icon') || '1';
                icon = icon === '1' ? '2' : '1';
                var src = staticUrl + '/img/login_icon' + icon + '.png';
                $(this).data('icon', icon).attr('src', src);
                $('.login-form').hide();
                $('#login' + icon).show();
            });
    }

    function _createLogin1(body) {
        var form = $('<div>').addClass('login-form').attr('id', 'login1').appendTo(body);
        var title = $('<div>').addClass('login-title').attr('id', 'ltLogin').append(Language.UserLogin).appendTo(form);
        if (showCaptcha) {
            title.css('padding-bottom', '0');
        }

        _createFormItem(form, 'userName', 'text', 'fa fa-user', Language.UserName);
        _createFormItem(form, 'password', 'password', 'fa fa-lock', Language.Password);

        if (showCaptcha) {
            _createCaptcha(form);
        }

        var item = $('<div>').addClass('login-form-item').appendTo(form);
        $('<button>').addClass('btn-primary')
            .attr('id', 'btnLogin')
            .html(Language.Login)
            .appendTo(item)
            .on('click', function () {
                _login($(this), false);
            });

        $('<div>').attr('id', 'message').appendTo(form);
    }

    function _createFormItem(form, id, type, icon, placeholder) {
        var item = $('<div>').addClass('login-form-item').appendTo(form);
        $('<label>').addClass(icon).attr('for', id).appendTo(item);
        $('<input>')
            .attr('type', type)
            .attr('id', id)
            .attr('placeholder', placeholder)
            .attr('autocomplete', 'off')
            .appendTo(item);
        return item;
    }

    function _createCaptcha(form) {
        var item = _createFormItem(form, 'captcha', 'text', 'fa fa-check-circle-o', Language.Captcha);
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

    function _createLogin2(body) {
        var form = $('<div>').addClass('login-form').attr('id', 'login2').appendTo(body);
        form.hide();
        $('<div>')
            .addClass('login-title')
            .attr('id', 'ltScan')
            .append(Language.ScanCode)
            .appendTo(form);
        var qrcode = $('<div>').addClass('login-qrcode').appendTo(form);
        $('<img>')
            .attr('src', staticUrl + '/img/qrcode.png')
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
            .append(copyrightInfo)
            .appendTo(footer);
    }

    function _login(obj, force) {
        $('.login-form-item input').removeClass('error');
        var userName = _getFormValue('userName');
        var password = _getFormValue('password');
        var captcha = _getFormValue('captcha');
        var cid = Utils.getClientId();
        if (userName === '' || password === '' || (showCaptcha && captcha === ''))
            return;

        var text = obj.html();
        var btn = obj.attr('disabled', true).html(Language.Logining);
        $.post(baseUrl + '/signin', {
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