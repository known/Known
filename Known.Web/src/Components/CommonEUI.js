/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

function EUILayout(id, dom) {
    var layout = $('<div>').appendTo(dom);
    $(layout).layout({ fit: true });
    LayoutBody.Resizer[id] = function () {
        $(layout).layout('resize');
    };
    return layout;
}

function EUITreeGrid(id, dom) {
    var grid = $('<table>').appendTo(dom);
    $(grid).treegrid({ fit: true });
    LayoutBody.Resizer[id] = function () {
        $(grid).treegrid('resize');
    };
    return grid;
}