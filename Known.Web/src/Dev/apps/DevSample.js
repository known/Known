function DevForm() {
    var tabs = new Tabs({
        items: [
            { name: '表单1', component: new DevTestForm('form') },
            { name: '表单2', component: new DevTestForm('form-block') }
        ]
    });

    this.render = function () {
        var elem = $('<div>').addClass('content');
        tabs.render().appendTo(elem);
        return elem;
    }

    function DevTestForm(type) {
        var SampleCodes = ['选项一', '选项二'];
        var form = new Form('DevForm', {
            type: type, tips: '带星号为必填字段。',
            fields: [
                { field: 'Id', type: 'hidden' },
                { title: '基本信息', type: 'group' },
                { title: '只读文本', field: 'Readonly', type: 'text', readonly: true },
                { title: '文本', field: 'Text', type: 'text', required: true },
                { title: '日期', field: 'Date', type: 'date', placeholder: DateFormat },
                { title: '下拉框', field: 'Select', type: 'select', code: SampleCodes },
                { title: '其他信息', type: 'group' },
                { title: '复选框', field: 'Check', type: 'checkbox', code: SampleCodes },
                { title: '单选框', field: 'Radio', type: 'radio', code: SampleCodes },
                { title: '文本', field: 'Text', type: 'text', unit: '元' },
                { title: '文本域', field: 'TextArea', type: 'textarea', itemStyle: 'block' }
            ],
            toolbar: [{
                icon: 'fa fa-save', text: '保存', handler: function (e) {
                    form.save('', function () {
                        app.router.back();
                    });
                }
            }]
        });

        this.render = function () {
            var elem = $('<div>').addClass('content');
            form.render().appendTo(elem);
            return elem;
        }

        this.mounted = function () {
            form.setData({
                Id: '1', Readonly: 'T0001', Text: 'test', Date: '2021-01-01',
                Select: '选项一', Check: '选项二', Radio: '选项一', Text1: 3000,
                TextArea: 'tetstest'
            });
        }
    }
}

function DevGrid() {
    var grid = new Grid({
        url: '', autoQuery: false,
        columns: [
            { title: '报警时间', field: 'AlarmTime' },
            { title: '参数', field: 'ParamName' },
            { title: '报警值', field: 'ParamValue' },
            { title: '报警阈值', field: 'AlarmLimit' }
        ],
        querys: [
            { title: '时间', field: 'Date', type: 'dateRange', labelWidth: '60px', width: '110px' }
        ],
        onSearch: function () {
            //grid.reload({ waterNo: currWater.Code });
        }
    });

    this.render = function () {
        var elem = $('<div>').addClass('content');
        grid.render().appendTo(elem);
        return elem;
    }

    this.mounted = function () {
        grid.setData([
            { AlarmTime: '2021-11-01 14:00:00', ParamName: '氨氮', ParamValue: '560mg/L', AlarmLimit: '0.05-500mg/L' },
            { AlarmTime: '2021-11-05 12:00:00', ParamName: '氨氮', ParamValue: '501mg/L', AlarmLimit: '0.05-500mg/L' }
        ]);
    }
}

function DevError() {
    //fields
    var menu = new Menu({
        items: [
            { bgColor: '#66cc66', icon: 'fa fa-close', name: '403', component: new Error({ type: '403' }) },
            { bgColor: '#224b39', icon: 'fa fa-close', name: '404', component: new Error({ type: '404' }) },
            { bgColor: '#0096dd', icon: 'fa fa-close', name: '500', component: new Error({ type: '500' }) },
        ]
    });

    //methods
    this.render = function () {
        var elem = $('<div>').addClass('content');
        menu.render().appendTo(elem);
        return elem;
    }
}

function IoTOnline() {
    this.render = function () {
        var time = new Date().format('yyyy-MM-dd HH:mm:ss');
        var elem = $('<div>').addClass('content param');
        $('<div>').addClass('param-name').html('监测参数：氨氮').appendTo(elem);
        var value = $('<div>').addClass('param-value').appendTo(elem);
        $('<span>').addClass('value').html('205').appendTo(value);
        $('<span>').addClass('unit').html('mg/L').appendTo(value);
        $('<div>').addClass('param-time').html('时间：' + time).appendTo(elem);
        $('<div>').addClass('param-time').html('阈值：0.05 - 500 mg/L').appendTo(elem);
        return elem;
    }
}

function IoTSetting() {
    var form = new Form('Setting', {
        type: 'form-block',
        fields: [
            { title: '报警阈值设置', type: 'group' },
            { title: '氨氮最大值', field: 'MaxAN', type: 'text', required: true, unit: 'mg/L' },
            { title: '短信提醒设置', type: 'group' },
            { title: '通知手机号', field: 'NoticePhone', type: 'text', required: true }
        ],
        toolbar: [{
            icon: 'fa fa-save', text: '保存', handler: function (e) {
                form.save('', function () {
                    app.router.back();
                });
            }
        }]
    });

    this.render = function () {
        var elem = $('<div>').addClass('content setting');
        form.render().appendTo(elem);
        return elem;
    }

    this.mounted = function () {
        form.setData({ MaxAN: '500', NoticePhone: '13812345678' });
    }
}