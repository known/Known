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

function BizFlowLog() {
    //fields
    var url = {
        QueryModels: BizFlow.url.GetFlowLogs
    };

    var grid = new Grid('BizFile', {
        url: url.QueryModels, querys: [], page: false, autoQuery: false, showCheckBox: false,
        columns: BizFlow.logColumns
    });

    //methods
    this.render = function () {
        return grid.render();
    }

    this.load = function (e) {
        grid.reload({ bizId: e.head.Id });
    }
}

var BizFlow = {

    url: {
        GetFlowTodos: baseUrl + '/System/GetFlowTodos',
        GetFlowLogs: baseUrl + '/System/GetFlowLogs',
        AssignFlow: baseUrl + '/System/AssignFlow',
        SubmitFlow: baseUrl + '/System/SubmitFlow',
        RevokeFlow: baseUrl + '/System/RevokeFlow',
        ReturnFlow: baseUrl + '/System/ReturnFlow',
        StopFlow: baseUrl +'/System/StopFlow'
    },
    logColumns: [
        { title: Language.ExecuteStep, field: 'StepName', width: '100px' },
        { title: Language.ExecuteBy, field: 'ExecuteBy', width: '100px' },
        { title: Language.ExecuteTime, field: 'ExecuteTime', width: '140px', placeholder: DateTimeFormat },
        {
            title: Language.ExecuteAction, field: 'Result', width: '100px', format: function (d) {
                return '<span class="badge">' + d.Result + '</span>';
            }
        },
        { title: Language.ExecuteContent, field: 'Note' }
    ],

    getStatusColor: function (status) {
        if (!status)
            return '';

        if (status.indexOf('暂存') > -1 || status.indexOf('关闭') > -1) {
            return ' gray';
        } else if (status.indexOf('撤回') > -1) {
            return ' warning';
        } else if (status.indexOf('退回') > -1 || status.indexOf('删除') > -1) {
            return ' danger';
        } else if (status.indexOf('通过') > -1 || status.indexOf('中') > -1) {
            return ' success';
        } else if (status.indexOf('待') > -1 || status.indexOf('已') > -1) {
            return ' info';
        }

        return '';
    },

    loadTodos: function (divId) {
        var _this = this;
        $('#' + divId).addClass('grid').html('<table id="gridFlowTask"></table>');
        new Grid('FlowTask', {
            url: _this.url.GetFlowTodos, page: false, fixed: false, showCheckBox: false,
            columns: [
                { title: Language.Type, field: 'FlowName', width: '100px' },
                {
                    title: Language.Status, field: 'BizStatus', width: '80px', format: function (d) {
                        var color = _this.getStatusColor(d.BizStatus);
                        return '<span class="badge' + color + '">' + d.BizStatus + '</span>';
                    }
                },
                {
                    title: Language.Task, field: 'BizName', format: function (d) {
                        var span = $('<span class="link">')
                            .data('url', d.BizUrl)
                            .html(d.BizName);
                        if (d.BizUrl.indexOf('js:') > -1) {
                            span.on('click', function () {
                                var script = $(this).data('url').substr(3);
                                eval('(' + script + ')');
                            });
                        } else {
                            span.on('click', function () {
                                location = baseUrl + $(this).data('url');
                            });
                        }
                        return span;
                    }
                }
            ]
        });
    },

    loadLogs: function (elem, bizId) {
        var el = typeof elem === 'string' ? $('#' + elem) : elem;
        el.addClass('flow-log').html('').append('<h2>' + Language.FlowRecord + '</h2>');
        var url = this.url.GetFlowLogs + '?bizId=' + bizId;
        var columns = [];
        if (Utils.checkMobile()) {
            columns = [{
                title: Language.FlowStep, field: 'StepName', format: function (d) {
                    var time = new Date(d.ExecuteTime).format(DateTimeFormat);
                    var html = '<div class="flow-name"><span class="flow-step">' + d.StepName + '</span>' + d.ExecuteBy + '（' + time + '）</div>';
                    html += '<div class="flow-note">' + d.Note + '</div>';
                    return html;
                }
            }];
            var grid = new Grid({ url: url, columns: columns });
            grid.render().appendTo(el);
        } else {
            el.append('<table id="gridFlowLog"></table>');
            new Grid('FlowLog', {
                page: false, fixed: false, showCheckBox: false,
                url: url, columns: this.logColumns
            });
        }
    },

    assignData: {},
    assign: function (bizId, callback) {
        var _this = this;
        this._openForm({
            title: Language.Assign, height: 250,
            fields: [
                { title: Language.AssignTo, field: 'UserName', type: 'picker', pick: { action: 'user' }, required: true },
                { title: Language.Note, field: 'Note', type: 'textarea' }
            ],
            data: _this.assignData,
            callback: function (e) {
                if (typeof bizId === 'function') {
                    bizId(e);
                } else {
                    Ajax.post(_this.url.AssignFlow, {
                        bizId: bizId, userName: e.data.UserName, note: e.data.Note
                    }, function () {
                        callback && callback();
                        e.layer.close();
                    });
                }
            }
        });
    },

    submitData: {},
    submit: function (bizId, callback) {
        var _this = this;
        this._openForm({
            title: Language.Submit, height: 250,
            fields: [
                { title: Language.SubmitTo, field: 'UserName', type: 'picker', pick: { action: 'user' }, required: true },
                { title: Language.Note, field: 'Note', type: 'textarea' }
            ],
            data: _this.submitData,
            callback: function (e) {
                if (typeof bizId === 'function') {
                    bizId(e);
                } else {
                    Ajax.post(_this.url.SubmitFlow, {
                        bizId: bizId, userName: e.data.UserName, note: e.data.Note
                    }, function () {
                        callback && callback();
                        e.layer.close();
                    });
                }
            }
        });
    },

    revoke: function (bizId, callback) {
        var _this = this;
        this._openForm({
            title: Language.Revoke, callback: function (e) {
                if (typeof bizId === 'function') {
                    bizId(e);
                } else {
                    Ajax.post(_this.url.RevokeFlow, {
                        bizId: bizId, reason: e.data.Reason
                    }, function () {
                        callback && callback();
                        e.layer.close();
                    });
                }
            }
        });
    },

    return: function (bizId, bizStatus, callback) {
        var _this = this;
        this._openForm({
            title: Language.Return, callback: function (e) {
                if (typeof bizId === 'function') {
                    bizId(e);
                } else {
                    Ajax.post(_this.url.ReturnFlow, {
                        bizId: bizId, bizStatus: bizStatus, reason: e.data.Reason
                    }, function () {
                        callback && callback();
                        e.layer.close();
                    });
                }
            }
        });
    },

    stop: function (bizId, callback) {
        var _this = this;
        this._openForm({
            title: Language.Stop, callback: function (e) {
                if (typeof bizId === 'function') {
                    bizId(e);
                } else {
                    Ajax.post(_this.url.StopFlow, {
                        bizId: bizId, reason: e.data.Reason
                    }, function () {
                        callback && callback();
                        e.layer.close();
                    });
                }
            }
        });
    },

    _openForm: function (option) {
        var form;
        if (!option.fields) {
            option.fields = [{
                title: option.title + Language.Reason, field: 'Reason', type: 'textarea', required: true
            }];
        }

        option.success = function (e) { form = e.form; };
        option.buttons = [{
            text: Language.OK, handler: function (e) {
                if (!form.validate())
                    return;

                var confirmText = option.confirmText || (Language.OK + option.title + '？');
                Layer.confirm(confirmText, function () {
                    var data = form.getData();
                    option.callback && option.callback({ layer: e, data: data });
                });
            }
        }];

        BizForm.open(option);
    }

}