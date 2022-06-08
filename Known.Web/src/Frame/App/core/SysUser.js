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

function SysUser(option) {
    //fields
    var list = new List({
        style: 'info',
        items: function () {
            return [
                { icon: 'fa fa-user', title: app.user.Name + '(' + app.user.UserName + ')' },
                { icon: 'fa fa-mobile', title: app.user.Mobile },
                { icon: 'fa fa-envelope-o', title: app.user.Email },
                { icon: 'fa fa-university', title: app.user.OrgName || app.user.CompName },
                {
                    icon: 'fa fa-lock', title: Language.UpdatePassword, onClick: function () {
                        app.route({ name: Language.UpdatePassword, component: new SysChangePwd(app.user) });
                    }
                }
            ];
        }
    });

    //methods
    this.render = function () {
        var elem = $('<div>').addClass('content user');
        var avatar = $('<div>').addClass('avatar').appendTo(elem);
        $('<img>').attr('src', staticUrl + app.user.AvatarUrl).appendTo(avatar);
        list.render().appendTo(elem);
        _createLogoutButton(elem);
        return elem;
    }

    //private
    function _createLogoutButton(dom) {
        var btn = $('<div>').addClass('btnLogout').appendTo(dom);
        $('<button>')
            .addClass('danger')
            .html(Language.SafeLogout)
            .appendTo(btn)
            .click(function () {
                app.logout();
            });
    }
}