/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-26     KnownChen
 * ------------------------------------------------------------------------------- */

function LayoutHead(body) {
    var lang = {
        RefreshPage: '刷新页面',
        ShareSystem: '分享系统',
        FullScreen: '全屏切换',
        SiteMap: '站点地图',
        UserCenter: '个人中心',
        SafeLogout: '安全退出',
        Collapse: '折叠/展开',
        TechSupport: '技术支持',
        Feedback: '问题反馈',
        LogoutConfirm: '确定要退出系统？'
    };
    var header, userPanel;

    this.render = function (dom) {
        header = $('<div>').addClass('kui-header').appendTo(dom);
        _createToggle();
        _createNavTool();
    }

    this.setMenus = function (menus) {
        _createTopMenu(menus);
    }

    this.setUser = function (user) {
        userPanel.Name.html(user.Name);
        userPanel.Avatar.attr('src', user.AvatarUrl);
    }

    //private
    function _createToggle() {
        var toggle = $('<div>').addClass('toggleMenu').appendTo(header);
        $('<i>').addClass('fa fa-dedent')
            .attr('title', lang.Collapse)
            .appendTo(toggle)
            .on('click', function () {
                _toggle($(this));
                body.resize();
            });
    }

    function _toggle(obj) {
        var status = obj.data('status');
        if (status && status === '1') {
            obj.data('status', '0').addClass('fa-dedent').removeClass('fa-indent');
            $('body').removeClass('kui-mini');
        } else {
            obj.data('status', '1').addClass('fa-indent').removeClass('fa-dedent');
            $('body').addClass('kui-mini');
            $('.nav-tree i').each(function (i, e) {
                var title = $(e).next().text();
                $(e).attr('title', title);
            });
        }
    }

    function _createTopMenu(menus) {

    }

    function _createNavTool() {
        var tool = $('<ul>').addClass('nav right').appendTo(header);
        _createToolItem(tool, lang.RefreshPage, 'fa fa-refresh', function () {
            body.refresh();
        });
        //_createToolItem(tool, lang.ShareSystem, 'fa fa-share-alt');
        //_createToolItem(tool, lang.Feedback, 'fa fa-commenting-o', function () {
        //
        //});
        _createToolItem(tool, lang.FullScreen, 'fa fa-arrows-alt', function () {
            _toogleFullScreen($(this));
        });
        //_createToolItem(tool, lang.Shortcut, 'fa fa-external-link');
        //_createToolItem(tool, lang.SiteMap, 'fa fa-sitemap');
        userPanel = _createUserPanel(tool);
    }

    function _createToolItem(parent, title, icon, action) {
        var li = $('<li>').addClass('nav-item')
            .attr('title', title)
            .on('click', action)
            .appendTo(parent);
        $('<i>').addClass(icon).appendTo(li);
    }

    function _createUserPanel(parent) {
        var dropdown = $('<li>').addClass('nav-item dropdown').appendTo(parent);
        var title = $('<div>').addClass('nav-title title').appendTo(dropdown);
        userAvatar = $('<img>')
            .attr('alt', 'Avatar')
            .attr('src', '/img/face1.png')
            .appendTo(title);
        userName = $('<span>').appendTo(title);
        $('<i>').addClass('fa fa-caret-down arrow').appendTo(title);

        var child = $('<dl>').addClass('nav-child child').appendTo(dropdown);
        $('<dd>')
            .append('<i class="fa fa-user">')
            .append($('<span>').html(lang.UserCenter))
            .appendTo(child)
            .on('click', function () {
                body.showPage({
                    code: 'UserInfo', name: lang.UserCenter, icon: 'fa fa-user'
                });
            });
        $('<dd>')
            .append('<i class="fa fa-power-off">')
            .append($('<span>').html(lang.SafeLogout))
            .appendTo(child).on('click', function () {
                Layer.confirm(lang.LogoutConfirm, function () {
                    $.post('/signout', function () {
                        location.reload();
                    });
                });
            });

        return { Avatar: userAvatar, Name: userName };
    }

    function _toogleFullScreen(obj) {
        if (obj.data('fullScreen') === '1') {
            _exitScreen(obj);
        } else {
            _fullScreen(obj);
        }
    }

    function _fullScreen(obj) {
        var el = document.documentElement;
        var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
        if (rfs) {
            rfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            //for IE
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
        obj.data('fullScreen', '1').html('<i class="fa fa-arrows">');
    }

    function _exitScreen(obj) {
        var el = document;
        var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
        if (cfs) {
            cfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            //for IE
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
        obj.data('fullScreen', '0').html('<i class="fa fa-arrows-alt">');
    }
}
