/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-27     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

function DevSample() {
    var menus = [
        {
            text: '表单', children: [
                { text: '普通表单', page: new CommonForm() },
                { text: '高级表单', page: new AdvanceForm() }
            ]
        },
        {
            text: '表格', children: [
                { text: '普通表格', page: new CommonGrid() }
            ]
        }
    ];

    //methods
    this.render = function (dom) {
        var layout = $('<div>').addClass('easyui-layout').appendTo(dom);
        $(layout).layout();
        $(layout).layout('add', { id: 'dsMenu', region: 'west', title: '示例菜单', width: '180px', split: true });
        $(layout).layout('add', { id: 'dsPage', region: 'center', title: '<span id="dsTitle">示例</span>' });
        _createMenu('#dsMenu');
        LayoutBody.Resizer.resizeDevSample = function () {
            $(layout).layout('resize');
        };
    }

    //private
    function _createMenu(dom) {
        var tree = $('<ul>').appendTo($(dom));
        $(tree).tree({
            data: menus,
            onClick: function (node) {
                $('#dsTitle').html('示例-' + node.text);
                node.page.render($('#dsPage'));
            }
        });
    }

    function CommonForm() {
        this.render = function (dom) {

        }
    }

    function AdvanceForm() {
        this.render = function (dom) {

        }
    }

    function CommonGrid() {
        this.render = function (dom) {

        }
    }
}

$.extend(Page, {
    DevSample: { component: new DevSample() }
});