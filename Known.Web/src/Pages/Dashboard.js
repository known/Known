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

function Dashboard() {
    //methods
    this.render = function (dom) {
        $('<h1>').html('欢迎使用').appendTo(dom);
    }

    //private
    
}

$.extend(Page, {
    Dashboard: { component: new Dashboard() }
});