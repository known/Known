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

function Card(dom, option) {
    var card = $('<div>').addClass('card').addClass(option.style).appendTo(dom);

    if (option.title) {
        var head = $('<div>').addClass('card-head').appendTo(card);
        if (option.icon)
            $('<i>').addClass(option.icon).appendTo(head);
        $('<span>').addClass('title').html(option.title).appendTo(head);

        if (option.tool) {
            var tool = $('<span>').addClass('tool').appendTo(head);
            option.tool(tool);
        }
    }

    if (option.body) {
        var body = $('<div>').addClass('card-body').appendTo(card);
        option.body(body);
    }

    if (option.foot) {
        var foot = $('<div>').addClass('card-foot').appendTo(card);
        option.foot(foot);
    }
}