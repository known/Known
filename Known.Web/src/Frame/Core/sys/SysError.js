function SysError() {
    var url = {
        QueryModels: baseUrl + '/System/QueryErrors',
        DeleteError: baseUrl + '/System/DeleteError'
    };
    var view = new View('SysError', {
        url: url,
        columns: [
            {
                title: Language.Operate, field: 'Id', width: '100px', format: function (d) {
                    var html = $('<span>');
                    _createLink(html, d, Language.Detail, function () {
                        _showInfo($(this).data('data'));
                    }).css('margin-right', '5px');
                    _createLink(html, d, Language.Remove, function () {
                        _delInfo($(this).data('data').Id);
                    });
                    return html;
                }
            },
            { title: Language.System, field: 'System', width: '80px', query: true },
            { title: Language.OperateBy, field: 'User', width: '80px', query: true },
            { title: Language.OperateTime, field: 'CreateTime', width: '140px', placeholder: DateTimeFormat },
            { title: Language.ErrorMessage, field: 'Message', width: '150px' },
            {
                title: 'IP', field: 'IP', width: '150px', format: function (d) {
                    return d.IP + ' - ' + d.IPName;
                }
            },
            { title: 'Url', field: 'Url' }
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
    function _showInfo(data) {
        Layer.open({
            title: Language.Error + Language.Detail, width: 1000, height: 400,
            content: '<pre>' + data.StackTrace + '</pre>'
        });
    }

    function _delInfo(id) {
        Layer.confirm(Language.ConfirmDelete, function () {
            Ajax.post(url.DeleteError, { id: id }, function () {
                view.load();
            });
        });
    }

    function _createLink(dom, data, text, callback) {
        return $('<span>')
            .addClass('link')
            .data('data', data)
            .html(text)
            .appendTo(dom)
            .on('click', callback);
    }
}

$.extend(Page, {
    SysError: { component: new SysError() }
});