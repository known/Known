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

function Dashboard() {
    //methods
    this.render = function (dom) {
        dom.css({ padding: '10px', backgroundColor: 'inherit' });
        _createWorkspace(dom);

        var row = $('<div>').addClass('row').appendTo(dom);
        _createMyToDoList(row);
        _createCommonFunction(row);
    }

    //private
    function _createWorkspace(dom) {
        if (!curUser) {
            curUser = {};
        }
        var card = $('<div>').addClass('card ws-card').appendTo(dom);
        $('<div>').addClass('ws-title').html('工作台').appendTo(card);
        $('<img>').addClass('ws-avatar')
            .attr('alt', 'Avatar')
            .attr('src', curUser.AvatarUrl)
            .appendTo(card);
        var text = '早安！' + curUser.Name + '，开始您一天的工作吧！';
        $('<span>').addClass('ws-name').html(text).appendTo(card);
        var time = (new Date()).format('yyyy年MM月dd日 www');
        $('<span>').addClass('ws-tips').html('今天是：' + time).appendTo(card);
    }

    function _createMyToDoList(dom) {
        Card(dom, {
            style: 'ws-todo', title: '我的待办', icon: 'fa fa-list',
            body: function (body) {

            }
        });
    }

    function _createCommonFunction(dom) {
        Card(dom, {
            style: 'ws-func', title: '常用功能', icon: 'fa fa-th',
            body: function (body) {

            }
        });
    }
}

$.extend(Page, {
    Dashboard: { component: new Dashboard() }
});