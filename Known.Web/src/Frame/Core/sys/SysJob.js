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

function SysJob() {
    var url = {
        QueryModels: baseUrl + '/System/QueryJobSettings',
        DeleteModels: baseUrl + '/System/DeleteJobSettings',
        SaveModel: baseUrl + '/System/SaveJobSetting'
    };
    var gridParam;

    var view = new View('JobSetting', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            {
                title: Language.CompNo, field: 'CompNo', width: '100px', query: true, format: function (d) {
                    var name = d.CompNo;
                    if (d.CompName) {
                        name += '-' + d.CompName;
                    }
                    return name;
                }, type: 'text', required: true
            },
            {
                title: Language.System, field: 'AppId', width: '100px', query: true, format: function (d) {
                    var name = d.AppId;
                    if (d.AppName) {
                        name += '-' + d.AppName;
                    }
                    return name;
                }, type: 'select', required: true, code: 'ProductData'
            },
            { title: Language.Name, field: 'Name', width: '100px', query: true, type: 'text', required: true },
            {
                title: 'Job' + Language.Type, field: 'TypeName', width: '100px', type: 'text', inputStyle: 'width:50%', lineBlock: true, required: true,
                inputHtml: '<div id="rblTypeName" style="display:inline-block;margin-left:5px;"></div>'
            },
            { title: Language.ExecuteTime, field: 'RunTime', width: '100px', type: 'text', tips: Language.Sample + '：HH:mm:ss=07:00:00' },
            { title: Language.ExecuteInterval, field: 'Interval', width: '100px', type: 'text', tips: Language.ExecuteIntervalTips },
            {
                title: Language.ExecuteStatus, field: 'Status', width: '100px', type: 'select', format: function (d) {
                    var color = ' gray';
                    if (d.Status === Language.Executing)
                        color = ' info';
                    else if (d.Status === Language.ExecutePass)
                        color = ' success';
                    else if (d.Status === Language.ExecuteFail)
                        color = ' danger';
                    return '<span class="badge' + color + '">' + d.Status + '</span>';
                }, query: true, readonly: true, code: [Language.NoExecute, Language.Executing, Language.ExecutePass, Language.ExecuteFail]
            },
            { title: Language.LastTime, field: 'LastTime', width: '140px', placeholder: DateTimeFormat, align: 'center' },
            { label: Language.Option, title: Language.Enable, field: 'Enabled', width: '100px', align: 'center', type: 'checkbox', required: true, code: 'YesNo' },
            {
                title: Language.ErrorMessage, field: 'Exception', format: function (d) {
                    if (!d.Exception)
                        return '';

                    return $('<span>')
                        .addClass('link')
                        .html(Language.Detail)
                        .data('note', d.Exception)
                        .on('click', function () {
                            var note = $(this).data('note');
                            Layer.open({
                                title: Language.Error + Language.Detail,
                                content: '<pre>' + note + '</pre>'
                            });
                        });
                }
            },
            { onlyForm: true, title: Language.ExecuteParams, field: 'Params', type: 'html', lineBlock: true, inputHtml: '<table id="gridParam" class="grid"></table>' }
        ],
        formOption: {
            data: { Id: '', Status: Language.NoExecute, Enabled: 1 },
            setData: function (e) {
                new Input($('#rblTypeName'), {
                    type: 'radio', field: 'JobType',
                    code: ['HttpJob', 'TaskJob'],
                    onClick: function (value) {
                        e.form.TypeName.setValue(value);
                    }
                });

                gridParam = new Grid('Param', {
                    edit: true,
                    columns: [
                        {
                            action: 'add', icon: 'fa fa-plus', align: 'center', width: '80px', actionFormat: function (d) {
                                return '<span class="link" action="remove">' + Language.Delete + '</span>';
                            }
                        },
                        { title: Language.ParameterName, field: 'Name', type: 'text', width: '150px' },
                        { title: Language.ParameterValue, field: 'Value', type: 'text' }
                    ]
                });
                var params = [], param = e.data.Params;
                if (param) {
                    for (var p in param) {
                        params.push({ Name: p, Value: param[p] });
                    }
                }
                gridParam.setData(params);
            },
            onSaving: function (d) {
                var params = gridParam.getData();
                if (params.length) {
                    d.Params = {};
                    for (var i = 0; i < params.length; i++) {
                        var pv = params[i];
                        d.Params[pv.Name] = pv.Value;
                    }
                }
            }
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load();
    }
}

$.extend(Page, {
    SysJob: { component: new SysJob() }
});