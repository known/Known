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

function SysPage() {
    //fields
    var url = {
        DeleteModels: baseUrl + '/System/DeleteModules',
        MoveModel: baseUrl + '/System/MoveModule',
        SaveModel: baseUrl + '/System/SaveModule'
    };
    var tree = new SysModuleTree({
        hideButton: true,
        onClick: function (node) {
            if (!node.children) {
                editor.load(node);
            }
        }
    });
    //var editor = new PageEditor({});

    //methods
    this.render = function (dom) {
        Utils.addJs('/libs/prettify/prettify.js');
        Utils.addJs('/libs/prettify/lang-css.js');

        var elem = $('<div>').addClass('fit').appendTo(dom);
        var left = $('<div>').addClass('fit-col-3').appendTo(elem);
        tree.render().appendTo(left);

        var right = $('<div>').addClass('fit-col-7').appendTo(elem);
        //editor.render(right);
    }

    this.mounted = function () {
        tree.reload();
    }
}

$.extend(Page, {
    SysPage: { component: new SysPage() }
});