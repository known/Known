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

function LayoutBody() {
    var tabs, curPage = { code: 'Dashboard', name: '首页' };

    //methods
    this.render = function (dom) {
        tabs = $('<div>')
            .addClass('kui-body easyui-tabs')
            .appendTo(dom);
        _createDashboard(tabs);
        $(tabs).tabs({
            onAdd: function (title, index) {
                var tab = $(this).tabs('getTab', title);
                var pageId = tab.attr('id');
                Admin.render(tab, pageId);
            },
            onSelect: function (title, index) {
                var tab = $(this).tabs('getTab', title);
                curPage.code = tab.attr('id');
                curPage.name = title;
            }
        });
    }

    this.resize = function () {
        $(tabs).tabs('resize');
        for (var p in LayoutBody.Resizer) {
            LayoutBody.Resizer[p]();
        }
    }

    this.refresh = function () {
        var tab = $(tabs).tabs('getTab', curPage.name);
        Admin.render(tab, curPage.code);
    }

    this.showPage = function (page) {
        var tab = $(tabs).tabs('getTab', page.name);
        if (tab && tab.length) {
            $(tabs).tabs('select', page.name);
        } else {
            _createTab(page);
        }
    }

    //private
    function _createDashboard(tabs) {
        var tab = $('<div>')
            .attr('id', curPage.code)
            .attr('title', curPage.name)
            .attr('iconCls', 'fa fa-home')
            .appendTo(tabs);
        Admin.render(tab, curPage.code);
    }

    function _createTab(page) {
        $(tabs).tabs('add', {
            id: page.code,
            title: page.name,
            iconCls: page.icon,
            closable: true
        });
    }
}

LayoutBody.Resizer = {};

$(window).resize(function () {
    for (var p in LayoutBody.Resizer) {
        LayoutBody.Resizer[p]();
    }
});