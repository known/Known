function SysLog() {
    var url = {
        QueryModels: baseUrl + '/System/QueryLogs'
    };

    var view = new View('SysLog', {
        url: url,
        columns: [
            { title: Language.OperateBy, field: 'CreateBy', width: '80px', query: true },
            { title: Language.OperateTime, field: 'CreateTime', width: '140px', placeholder: DateTimeFormat },
            { title: Language.LogType, field: 'Type', width: '100px' },
            { title: Language.LogTarget, field: 'Target', width: '100px' },
            { title: Language.LogContent, field: 'Content', query: true }
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
    SysLog: { component: new SysLog() }
});