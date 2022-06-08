function IconColumn() {
    this.field = {
        onlyForm: true, title: '图标', field: 'Icon', inputHtml: function (e) {
            $('<span>').addClass('field').appendTo(e.parent);
        }, inputStyle: 'icon-input', type: 'picker', pick: {
            action: 'icon', callback: function (e) {
                _setIcon(e.value);
            }
        }, required: true
    };

    this.setIcon = function (icon) {
        _setIcon(icon);
    }

    function _setIcon(icon) {
        $('.icon-input .field').attr('class', 'field ' + icon);
    }
}

function SysApp() {
    var url = {
        QueryModels: baseUrl + '/System/QueryApps',
        DeleteModels: baseUrl + '/System/DeleteApps',
        SaveModel: baseUrl + '/System/SaveApp'
    };
    var colIcon = new IconColumn();
    var view = new View('App', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            { title: '编码', field: 'Code', width: '80px', type: 'text', required: true },
            {
                title: '名称', field: 'Name', width: '120px', type: 'text', required: true, query: true, format: function (d, e) {
                    $('<i>').addClass(d.Icon).appendTo(e.td);
                    $('<a>').attr('target', '_blank')
                        .attr('href', baseUrl + '/?a=' + d.Code)
                        .html(d.Name)
                        .appendTo(e.td);
                }
            },
            colIcon.field,
            { label: '选项', title: '状态', field: 'Enabled', width: '80px', type: 'checkbox', code: 'Enabled', required: true, align: 'center' },
            { label: '选项', title: 'App', field: 'Extension.App', width: '80px', type: 'checkbox', code: 'YesNo', required: true, align: 'center' },
            { title: '顺序', field: 'Sort', width: '50px', type: 'text', required: true, align: 'center' },
            { title: '备注', field: 'Note', type: 'textarea', lineBlock: true }
        ],
        formOption: {
            style: 'form-block',
            data: { Id: '', Enabled: 1 },
            setData: function (e) {
                colIcon.setIcon(e.data.Icon);
            }
        },
        gridOption: {
            sortField: 'Sort', sortOrder: 'asc'
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
    SysApp: { component: new SysApp() }
});