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

var Log = {

    url: {
        GetLogs: baseUrl + '/System/GetLogs'
    },

    open: function (option) {
        var _this = this, elem = $('<div>');
        Layer.open({
            title: option.title || Language.OperateLog,
            width: option.width,
            height: option.height,
            content: elem,
            success: function () {
                _this.load(elem, option.bizId);
            }
        });
    },

    load: function (elem, bizId) {
        var el = typeof elem === 'string' ? $('#' + elem) : elem;
        el.addClass('log').html('');
        $('<h2>').html(Language.OperateLog).appendTo(el);
        var url = this.url.GetLogs + '?bizId=' + bizId;
        var columns = [
            { title: Language.OperateType, field: 'Type', width: '100px' },
            { title: Language.OperateBy, field: 'CreateBy', width: '100px' },
            { title: Language.OperateTime, field: 'CreateTime', width: '140px', placeholder: DateTimeFormat },
            { title: Language.OperateContent, field: 'Content' }
        ];
        $('<table>').attr('id', 'gridSysLog').appendTo(el);
        new Grid('SysLog', {
            page: false, fixed: false, showCheckBox: false,
            url: url, columns: columns
        });
    }

}