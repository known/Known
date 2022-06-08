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

const CommTypes = ['办公用品', '仪器'];
const CommUnits = ['个', '把', '套'];
const SampleCodes = ['选项一', '选项二'];

function DevSample() {
    var tabs = new Tabs({
        fit: true,
        items: [
            { name: '表单示例', component: new DevSampleForm() },
            { name: '表格示例', component: new DevSampleGrid() },
            { name: '其他示例', component: new DevSampleOther() },
            { name: 'Socket示例', component: new DevSampleSocket() }
        ]
    });

    //methods
    this.render = function (dom) {
        var elem = $('<div>').addClass('card-fit').appendTo(dom);
        tabs.render().appendTo(elem);
    }

    this.mounted = function () {
    }
}

Picker.action.commodity = {
    title: '选择商品', width: 800, formWidth: 500, formHeight: 300,
    url: baseUrl + '/System/QueryUsers',
    saveUrl: baseUrl + '/System/SaveUser', formData: { Name: 'test' },
    columns: [
        { title: '商品类别', field: 'Type', query: true, type: 'select', required: true, code: CommTypes },
        { title: '商品编码', field: 'UserName', query: true },
        { title: '商品名称', field: 'Name', query: true, type: 'text', required: true },
        { title: '规格型号', field: 'Model', type: 'text', inputBlock: true },
        { title: '产地', field: 'Place', type: 'text' },
        { title: '计量单位', field: 'Unit', type: 'select', required: true, code: CommUnits }
    ],
    valueField: 'UserName', textField: 'Name'
}

Picker.action.form = {
    title: '选择数据', width: 800,
    url: baseUrl + '/System/QueryUsers',
    columns: [
        { title: '商品类别', field: 'Type', query: true, code: CommTypes },
        { title: '商品编码', field: 'UserName', query: true },
        { title: '商品名称', field: 'Name', query: true },
        { title: '规格型号', field: 'Model' },
        { title: '产地', field: 'Place' },
        { title: '计量单位', field: 'Unit', code: CommUnits }
    ],
    valueField: 'UserName', textField: 'Name',
    onAddClick: function (e, callback) {
        var form = new DevSampleForm();
        form.showForm2({});
    }
}

function DevSampleForm() {
    var data = {
        Id: '1', Readonly: 'T0001', Text: 'test', Date: '2021-01-01',
        Select: '选项一', Check: '选项二', Radio: '选项一',
        Picker: 'System', PickerName: '超级管理员', Text1: 'testtt', Text2: 3000,
        TextArea: 'tetstest', Editor: '测试刚刚', Province: '江苏省', City: '苏州市', Area: '虎丘区',
        Lists: [
            { Code: '1', Name: '手机', Model: 'HONOR 20', Qty: 1, Unit: '台', Price: 2600, Amount: 2600, ListNote: '2019年' },
            { Code: '2', Name: '电脑', Model: 'HONOR Magic 14', Qty: 1, Unit: '台', Price: 4600, Amount: 4600, ListNote: '2018年' }
        ]
    };

    var form = new Form('Form', {
        style: 'form3',
        fields: [
            { field: 'Id', type: 'hidden' },
            { title: '只读文本', field: 'Readonly', type: 'text', readonly: true },
            { title: '文本', field: 'Text', type: 'text', required: true },
            { title: '日期', field: 'Date', type: 'date', placeholder: DateFormat },
            { title: '下拉框', field: 'Select', type: 'select', code: SampleCodes },
            { title: '复选框', field: 'Check', type: 'checkbox', code: SampleCodes },
            { title: '单选框', field: 'Radio', type: 'radio', code: SampleCodes },
            {
                title: '选择框', field: 'Picker', type: 'picker', pick: {
                    action: 'user', callback: function (e) { log(e); }
                }
            },
            { title: '文本', field: 'Text1', type: 'text', colSpan: 'col2', tips: '这个是跨了2列的字段，设置colSpan:col2' },
            { title: '文本', field: 'Text2', type: 'text', unit: '元' },
            { title: '所在省', field: 'Province', type: 'select' },
            { title: '所在市', field: 'City', type: 'select' },
            { title: '所在区县', field: 'Area', type: 'select' },
            {
                title: '经度', field: 'Longitude', type: 'text', label: '位置',
                inputStyle: 'width:88px;margin-right:5px;', placeholder: '经度', required: true
            },
            {
                title: '纬度', field: 'Latitude', type: 'text', label: '位置',
                inputStyle: 'width:88px;margin-right:5px;', placeholder: '纬度', required: true,
                inputHtml: KBMap.createSelectMapButton
            },
            { title: '文本域', field: 'TextArea', type: 'textarea', itemStyle: 'block' }
        ],
        setData: function (e) {
            Utils.initPCAField(e);
            e.form.Check.setData(['测试仪', '分析仪']);
            e.form.Check.setValue('分析仪');
        },
        toolbar: [
            {
                text: '保存', icon: 'fa fa-save', handler: function (e) {
                    log(e);
                    log(e.form.getData());
                }
            }
        ]
    });

    var form1 = new Form('Form1', {
        card: true, title: 'Form1', style: 'form3', width: 1024, height: 400,
        fields: [
            { field: 'Id', type: 'hidden' },
            { title: '只读文本', field: 'Readonly', type: 'text', readonly: true },
            { title: '文本', field: 'Text', type: 'text', required: true },
            { title: '日期', field: 'Date', type: 'date', placeholder: DateFormat },
            { title: '下拉框', field: 'Select', type: 'select', code: SampleCodes },
            { title: '复选框', field: 'Check', type: 'checkbox', code: SampleCodes },
            { title: '单选框', field: 'Radio', type: 'radio', code: SampleCodes },
            {
                title: '选择框', field: 'Picker', type: 'picker', pick: {
                    action: 'form', callback: function (e) { log(e); }
                }
            },
            { title: '文本', field: 'Text1', type: 'text', colSpan: 'col2', tips: '这个是跨了2列的字段，设置colSpan:col2' },
            { title: '文本', field: 'Text2', type: 'text', unit: '元' },
            { title: '文本域', field: 'TextArea', type: 'textarea', itemStyle: 'block' },
            { title: '富文本', field: 'Editor', type: 'editor', itemStyle: 'block' }
        ],
        toolbar: [
            {
                text: '保存', icon: 'fa fa-save', handler: function (e) {
                    form1.save('test', function (d) {
                        log(d);
                        Layer.tips('保存成功！');
                    });
                }
            },
            { text: '打印', icon: 'fa fa-print', handler: function (e) { log(e); }, detail: true }
        ]
    });

    var gridList = new Grid('BillList', {
        edit: true, fixed: false, showCheckBox: false,
        columns: [
            { field: 'Id', type: 'hidden' },
            { field: 'Name', type: 'hidden' },
            { action: 'add', icon: 'fa fa-plus', align: 'center', width: '80px', aFormat: 'remove,up,down' },
            {
                title: '商品名称', field: 'Code', type: 'picker', pick: {
                    action: 'commodity', callback: function (e) {
                        log(e);
                        e.form.Name.setValue(e.data.Name);
                        e.form.Model.setValue(e.data.Email);
                        e.form.Qty.setUnit('台');
                    }
                }, required: true, textField: 'Name'
            },
            { title: '规格型号', field: 'Model', type: 'text' },
            { title: '类型', field: 'Type', type: 'select', code: ['Test1', 'Test2'] },
            {
                title: '数量', field: 'Qty', type: 'text', required: true, change: function (e) {
                    var price = e.form.Price.getValue();
                    var amount = price * e.value;
                    e.form.Amount.setValue(amount);
                }, unit: ' ', unitField: 'Unit'
            },
            {
                title: '单价', field: 'Price', type: 'text', change: function (e) {
                    var qty = e.form.Qty.getValue();
                    var amount = qty * e.value;
                    e.form.Amount.setValue(amount);
                }, unit: '元'
            },
            {
                title: '金额', field: 'Amount', type: 'text', change: function (e) {
                    var qty = e.form.Qty.getValue();
                    if (qty > 0) {
                        var price = e.value / qty;
                        e.form.Price.setValue(price);
                    }
                }, unit: '元'
            },
            { title: '备注', field: 'ListNote', type: 'text' }
        ]
    });
    var form2 = new Form('Form2', {
        card: true, title: 'Form2',
        fields: [
            { field: 'Id', type: 'hidden' },
            { title: '只读文本', field: 'Readonly', type: 'text', readonly: true },
            { title: '文本', field: 'Text', type: 'text', required: true },
            { title: '备注', field: 'TextArea', type: 'text', itemStyle: 'block' },
            { title: '货物明细', field: 'Lists', type: 'component', itemStyle: 'list', component: gridList }
        ],
        toolbar: [
            {
                text: '保存', icon: 'fa fa-save', handler: function (e) {
                    log(e.form.getData());
                    //form2.save('test', function (d) {
                    //    log(d);
                    //    Layer.tips('保存成功！');
                    //});
                }
            },
            {
                text: '打印', icon: 'fa fa-print', handler: function (e) {
                    log(e);
                    Layer.open({
                        title: '打印', width: 880, height: 400,
                        content: '<div id="divPrint" style="height:100%"></div?',
                        success: function () {
                            BizPreview.pdf('divPrint', sysBaseUrl + '/Test/GetTestPdf');
                        }
                    });
                }
            }
        ]
    });

    var form3 = new Form('Form3', {
        width: 1024, height: 400,
        fields: [
            { field: 'Id', type: 'hidden' },
            { title: '只读文本', field: 'Readonly', type: 'text', readonly: true },
            { title: '文本', field: 'Text', type: 'text', required: true },
            { title: '下拉框', field: 'Select', type: 'select', code: SampleCodes },
            { title: '复选框', field: 'Check', type: 'checkbox', code: SampleCodes },
            { title: '单选框', field: 'Radio', type: 'radio', code: SampleCodes },
            {
                title: '选择框', field: 'Picker', type: 'picker', pick: {
                    action: 'user', callback: function (e) { log(e); }
                }
            },
            { title: '文本', field: 'Text1', type: 'text', tips: '这个是字段提示' },
            { title: '文本', field: 'Text2', type: 'text', unit: '元' },
            { title: '文本域', field: 'TextArea', type: 'textarea', itemStyle: 'block' }
        ],
        toolbar: [
            {
                text: '保存', icon: 'fa fa-save', handler: function (e) {
                    log(e);
                    Layer.tips('保存成功！');
                }
            }
        ],
        tabs: [
            { name: '基本信息' },
            { name: '附件信息', component: new BizFile('表单附件') }
        ]
    });

    var toolbar = new Toolbar({
        buttons: [
            { text: '单一表单', icon: 'fa fa-plus', handler: function (e) { form1.showDialog(data); } },
            { text: '单一表单(只读)', icon: 'fa fa-file-text-o', handler: function (e) { form1.showDialog(data, true); } },
            { text: '表头表体表单', icon: 'fa fa-plus', handler: function (e) { form2.showDialog(data); } },
            { text: '表头表体表单(只读)', icon: 'fa fa-file-text-o', handler: function (e) { form2.showDialog(data, true); } },
            { text: '选项卡表单', icon: 'fa fa-plus', handler: function (e) { form3.showDialog(data); } },
            { text: '选项卡表单(只读)', icon: 'fa fa-file-text-o', handler: function (e) { form3.showDialog(data, true); } }
        ]
    });

    //methods
    this.render = function () {
        var elem = $('<div>');

        toolbar.render().appendTo(elem);
        elem.append('<div class="sample-title">表单一</div>');
        form.render().appendTo(elem);
        form1.render().appendTo($('#app'));
        form2.render().appendTo($('#app'));
        form3.render().appendTo($('#app'));

        return elem;
    }

    this.mounted = function () {
        form.setData(data);
    }

    this.showForm2 = function (data) {
        form2.render().appendTo($('#app'));
        form2.show(data);
    }

    //private
}

function DevSampleGrid() {
    var view = new View('View', {
        url: { },
        columns: [
            { field: 'Id', type: 'hidden' },
            { title: '只读文本', field: 'Readonly', type: 'text', readonly: true, export: true },
            { title: '文本', field: 'Text', type: 'text', required: true, format: 'detail', query: true, import: true, export: true },
            { title: '日期', field: 'Date', type: 'date', placeholder: DateFormat, import: true, export: true },
            { title: '下拉框', field: 'Select', type: 'select', code: [{ Code: '1', Name:'选项一'}, '选项二'], query: true, import: true, export: true },
            { title: '复选框', field: 'Check', type: 'checkbox', code: SampleCodes, import: true, export: true },
            { title: '单选框', field: 'Radio', type: 'radio', code: SampleCodes, import: true, export: true },
            {
                title: '选择框', field: 'Picker', type: 'picker', pick: {
                    action: 'user', callback: function (e) { log(e); }
                }
            },
            { title: '文本', field: 'Text2', type: 'text', unit: '元', import: true },
            { title: '文本域', field: 'TextArea', type: 'textarea', itemStyle: 'block' }
        ],
        formOption: { max: true },
        gridOption: {
            autoQuery: false,
            formUrl: baseUrl + '/test', toolButtons: ['add', 'edit', 'remove', 'import']
        }
    });

    //methods
    this.render = function () {
        return view.render();
    }

    this.mounted = function () {
        view.setGridData([
            {
                Id: '1', Readonly: 'T0001', Text: 'test', Date: '2021-01-01',
                Select: '选项一', Check: '选项二', Radio: '选项一',
                Picker: 'System', PickerName: '超级管理员', Text1: 'testtt', Text2: 3000,
                TextArea: 'tetstest', Editor: '测试刚刚'
            }
        ]);
    }

    //private

}

function DevSampleOther() {
    var toolbar = new Toolbar({
        buttons: [
            { text: '500', handler: function (e) { _showError({ type: '500' }); } },
            { text: '403', handler: function (e) { _showError({ type: '403' }); } },
            { text: '404', handler: function (e) { _showError({ type: '404' }); } },
            { text: 'Error', handler: function (e) { _showError({ type: 'Test', title: '自定义错误', content: '错误内容' }); } },
            { text: '地图', handler: function (e) { _showMap(); } },
            { text: '添加顶部图标按钮', handler: function (e) { _addTopButton('test1', 'fa fa-user', ''); } },
            { text: '添加顶部文本按钮', handler: function (e) { _addTopButton('test2', '', '测试'); } },
            {
                text: '设置缓存', handler: function (e) {
                    $.post(sysBaseUrl + '/Test/SetCache?value=test', function (res) {
                        Layer.tips(res.Message);
                    });
                }
            },
            {
                text: '获取缓存', handler: function (e) {
                    $.get(sysBaseUrl + '/Test/GetCache', function (res) {
                        Layer.alert(res);
                    });
                }
            },
            {
                text: '打开最大化窗口', handler: function (e) {
                    Layer.open({ title: '最大化窗口', max: true });
                }
            }
        ]
    });

    var tbLayer = new Toolbar({
        buttons: [
            { text: 'open', handler: function (e) { Layer.open({ title: '标题', width: 300, height: 200, content: '测试' }); } },
            { text: 'loading', handler: function (e) { Layer.loading('数据提交中......'); } },
            { text: 'tips', handler: function (e) { Layer.tips('保存成功！'); } },
            { text: 'alert', handler: function (e) { Layer.alert('输入错误！', function () { }); } },
            { text: 'confirm', handler: function (e) { Layer.confirm('确定要删除？', function () { }); } }
        ]
    });

    //methods
    this.render = function () {
        var elem = $('<div>');
        toolbar.render().appendTo(elem);
        tbLayer.render().appendTo(elem);
        $('<div>').attr('id', 'map').css({ width: '600px', height: '200px' }).appendTo(elem);
        return elem;
    }

    this.mounted = function () {

    }

    //private
    function _showError(option) {
        app.route({ component: new Error(option) });
    }

    function _showMap() {
        KBMap.init('map', {
            callback: function (map) {
                var point = new BMap.Point(120, 30);
                map.centerAndZoom(point, 10);
                map.enableScrollWheelZoom(true);
                map.addControl(new BMap.NavigationControl());
                map.addControl(new BMap.ScaleControl());
                map.addControl(new BMap.OverviewMapControl());
                map.addControl(new BMap.MapTypeControl());
                map.setCurrentCity('苏州市');
            }
        });
    }

    function _addTopButton(id, icon, text) {
        top.Admin.prependNav({
            id: id, icon: icon, text: text, handler: function () {
                Layer.alert(icon + text);
            }
        });
    }
}

function DevSampleSocket() {
    var ws = new WsUtil();
    var toolbar = new Toolbar({
        buttons: [
            { text: '开启', handler: function (e) { _openWS(); } },
            { text: '关闭', handler: function (e) { _closeWS(); } }
        ]
    });
    var timer;

    //methods
    this.render = function () {
        var elem = $('<div>');
        toolbar.render().appendTo(elem);
        $('<div>').attr('id', 'wsTest').appendTo(elem);
        return elem;
    }

    this.mounted = function () {
        
    }

    this.destroy = function () {
        ws.close();
    }

    //private
    function _openWS() {
        //ws.connectById('DEVRealData', { Code: 'test', Name: '测试' });
        ws.connectById('DEVRealData1', { Code: 'test1', Name: '测试1' }, function (data) {
            _writeLog('接收：' + JSON.stringify(data));
        });
        _writeLog('打开连接');
        timer = setInterval(function () {
            var msg = JSON.stringify({ Test: Math.random() });
            ws.send(msg);
            _writeLog('发送：' + msg);
        }, 8000);
    }

    function _closeWS() {
        ws.close();
        clearInterval(timer);
        _writeLog('关闭连接');
    }

    function _writeLog(msg) {
        $('#wsTest').append(msg + '<br>');
    }
}

$.extend(Page, {
    DevSample: { component: new DevSample() }
});