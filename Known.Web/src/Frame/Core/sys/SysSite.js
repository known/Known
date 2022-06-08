function SysSite() {
    var url = {
        QueryModels: baseUrl + '/System/QuerySites',
        DeleteModels: baseUrl + '/System/DeleteSites',
        SaveModel: baseUrl + '/System/SaveSite'
    };

    var view = new View('SysSite', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            {
                title: 'Host', field: 'Host', type: 'text', required: true, format: function (d) {
                    return '<a href="http://' + d.Host + '" target="_blank">' + d.Host + '</i>';
                }
            },
            { title: 'AppId', field: 'AppId', type: 'text', required: true },
            {
                title: '图标', field: 'AppIcon', type: 'text', required: true, format: function (d) {
                    var color = '#' + Math.random().toString(16).substr(2, 6);
                    return '<i class="' + d.AppIcon + '" style="color:' + color + '"></i>';
                }
            },
            { title: 'App名称', field: 'AppName', type: 'text', inputStyle: 'width:300px', required: true },
            { title: '企业编码', field: 'CompNo', type: 'text', required: true },
            { title: '企业名称', field: 'CompName', type: 'text', inputStyle: 'width:300px', required: true },
            { title: '技术支持', field: 'SupportName', type: 'text', inputStyle: 'width:300px' },
            { title: '技术支持URL', field: 'SupportUrl', type: 'text', inputStyle: 'width:300px' },
            { title: '登录背景图', field: 'LoginImage', type: 'text', inputStyle: 'width:300px' },
            { label: '选项', title: '状态', field: 'Enabled', type: 'checkbox', required: true, align: 'center', code: 'Enabled' },
            { label: '选项', title: '顶部菜单', field: 'TopMenu', type: 'checkbox', required: true, align: 'center', code: 'YesNo' },
            { title: '备注', field: 'Note', type: 'textarea', lineBlock: true }
        ],
        formOption: {
            data: { Id: '', Enabled: 1 }
        },
        gridOption: {
            autoQuery: false, page: false,
            toolButtons: ['add', {
                name: '复制', icon: 'fa fa-copy', handler: function (e) {
                    e.selectRow(function (e) {
                        var data = $.extend({}, e.row, { Id: '' });
                        e.form.show(data);
                    });
                }
            }, 'edit', 'remove']
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
}

$.extend(Page, {
    SysSite: { component: new SysSite() }
});