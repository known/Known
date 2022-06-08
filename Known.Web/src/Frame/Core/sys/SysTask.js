function SysTask() {
    var url = {
        QueryModels: baseUrl + '/System/QueryTasks'
    };

    var view = new View('SysTask', {
        url: url,
        columns: [
            { title: Language.Type, field: 'Type', width: '100px', query: true },
            { title: Language.Name, field: 'Name', width: '200px', query: true },
            { title: Language.ExecuteTarget, field: 'Target', width: '200px' },
            {
                title: Language.ExecuteStatus, field: 'Status', width: '100px', format: function (d) {
                    var color = ' gray';
                    if (d.Status === Language.Executing)
                        color = ' info';
                    else if (d.Status === Language.ExecutePass)
                        color = ' success';
                    else if (d.Status === Language.ExecuteFail)
                        color = ' danger';
                    return '<span class="badge' + color + '">' + d.Status + '</span>';
                }, query: true, code: [Language.PendingExecute, Language.Executing, Language.ExecutePass, Language.ExecuteFail]
            },
            { title: Language.BeginTime, field: 'BeginTime', width: '140px', placeholder: DateTimeFormat },
            { title: Language.EndTime, field: 'EndTime', width: '140px', placeholder: DateTimeFormat },
            {
                title: Language.Note, field: 'Note', format: function (d) {
                    if (!d.Note)
                        return '';

                    return $('<span class="link">' + Language.Detail + '</span>').data('note', d.Note).on('click', function () {
                        var note = $(this).data('note');
                        Layer.open({
                            title: Language.Error + Language.Detail,
                            content: '<pre>' + note + '</pre>'
                        });
                    });
                }
            }
        ],
        gridOption: {
            sortField: 'CreateTime', sortOrder: 'desc',
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
    SysTask: { component: new SysTask() }
});