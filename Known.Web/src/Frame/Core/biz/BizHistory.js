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

var BizHistory = {

    url: {
        GetUserHistory: baseUrl + '/Home/GetUserHistory'
    },
    data: {},

    init: function (option) {
        var _this = this;
        $.get(_this.url.GetUserHistory, function (res) {
            if (res) {
                if ($.isPlainObject(res)) {
                    _this.data = res;
                } else {
                    _this.data = JSON.parse(res);
                }
            }
        });
    }

}