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

function Home() {
    var body = new LayoutBody();
    var head = new LayoutHead(body);
    var side = new LayoutSide(body);

    //methods
    this.render = function (dom) {
        head.render(dom);
        side.render(dom);
        body.render(dom);
    }

    this.mounted = function () {
        $.get('/Home/GetUserData?type=1', function (res) {
            if (!res.user) {
                location.reload();
            } else {
                Utils.setUser(res.user);
                Utils.setCodes(res.codes);
                //head.createMenu(res.)
                head.setUser(res.user);
                side.setMenus(res.menus);
            }
        });
    }
}

Layer.open = function (option) {
};

Layer.loading = function (message) {
};

Layer.tips = function (message) {
    $.messager.show({ msg: message, timeout: 3000 });
};

Layer.alert = function (message, callback) {
    $.messager.alert('提示', message, 'info', callback);
};

Layer.confirm = function (message, callback) {
    $.messager.confirm('确认', message, function (r) {
        if (r) {
            callback && callback();
        }
    });
};

app.route({ component: new Home() });