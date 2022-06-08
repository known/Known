function SysServer() {
    var url = {
        QueryModels: baseUrl + '/System/QueryServers'
    };
    var view = new View('SysServer', {
        url: url,
        columns: [
            { title: Language.Customer, field: 'CompName', query: true },
            { title: Language.System, field: 'AppId', query: true, code: 'ProductData' },
            { title: Language.Service, field: 'Type' },
            {
                title: Language.Status, field: 'Status', format: function (d) {
                    var color = ' gray';
                    if (d.Status === Language.Normal)
                        color = ' success';
                    return '<span class="badge' + color + '">' + d.Status + '</span>';
                }, code: [Language.BreakOff, Language.Normal]
            },
            {
                title: Language.ThreadCount, field: 'ThreadCount', format: function (d) {
                    return $('<span class="link">')
                        .html(d.ThreadCount)
                        .data('item', d)
                        .on('click', function () {
                            var item = $(this).data('item');
                            _showThreadInfo(item);
                        });
                }
            },
            { title: Language.UpdateTime, field: 'UpdateTime', width: '140px', placeholder: DateTimeFormat },
            { title: Language.StartTime, field: 'CreateTime', width: '140px', placeholder: DateTimeFormat },
            {
                title: 'IP', field: 'IP', width: '150px', format: function (d) {
                    return d.IP + ' - ' + d.IPName;
                }
            },
            { title: 'MAC' + Language.Address, field: 'MacAddress' }
        ],
        gridOption: {
            page: false
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load();
    }

    //private
    function _showThreadInfo(info) {
        var html = '<div class="fit"><table id="gridThreadInfo" class="grid"></table></div>';
        Layer.open({
            title: Language.ThreadInfo + ' - ' + info.Type,
            width: 550, height: 300,
            content: html,
            success: function () {
                var grid = new Grid('ThreadInfo', {
                    columns: [
                        { title: 'ID', field: 'Id' },
                        { title: Language.Name, field: 'Name' },
                        {
                            title: Language.Status, field: 'Status', format: function (d) {
                                var color = ' gray';
                                if (d.Status.indexOf('正常') > -1 || d.Status.indexOf('已') > -1)
                                    color = ' success';
                                else if (d.Status.indexOf('异常') > -1)
                                    color = ' danger';
                                return '<span class="badge' + color + '">' + d.Status + '</span>';
                            }
                        },
                        { title: Language.Description, field: 'Description' },
                        { title: Language.ErrorMessage, field: 'ErrorMessage' }
                    ]
                });
                grid.setData(info.Threads);
            }
        });
    }
}

$.extend(Page, {
    SysServer: { component: new SysServer() }
});