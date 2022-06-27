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

var Admin = {
    Menus: [],
    Buttons: [],
    render: function (elem, pageId) {
        var page = Page[pageId];
        if (!page) {
            page = { component: pageId + ' is not exist!' };
        }

        var router = new Router(elem);
        router.route(page);
    }
};