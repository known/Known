function DevCode() {
    //fields
    var url = {
        GetModels: sysBaseUrl + '/Develop/GetModels'
    };

    var tree = new Tree('tree', {
        url: url.GetModels,
        onClick: function (node) {
            if (!node.children) {
                form.setModel(node.id);
            }
        }
    });

    var form = new DevCodeForm(tree);

    var tabs = new Tabs({
        fit: true, renderAllTab: true,
        items: [
            { name: '领域模型', component: form },
            { name: '前端代码', component: '<pre id="codeView" class="code">' },
            { name: '实体代码', component: '<pre id="codeEntity" class="code">' },
            { name: '控制器代码', component: '<pre id="codeController" class="code">' },
            { name: '业务逻辑层', component: '<pre id="codeService" class="code">' },
            { name: '数据访问层', component: '<pre id="codeRepository" class="code">' },
            { name: '数据库脚本', component: '<pre id="codeSql" class="code">' }
        ]
    });

    //methods
    this.render = function (dom) {
        Utils.addJs('/libs/prettify/prettify.js');
        Utils.addJs('/libs/prettify/lang-css.js');

        var fit = $('<div>').addClass('fit').appendTo(dom);
        var left = $('<div>').addClass('fit-col-3').appendTo(fit);
        tree.render().appendTo(left);
        var right = $('<div>').addClass('fit-col-7').appendTo(fit);
        tabs.render().appendTo(right);
    }

    this.mounted = function () {
        form.bindEnablePaste();
    }
}

function DevCodeForm(tree) {
    //fields
    var url = {
        GetModel: sysBaseUrl + '/Develop/GetModel',
        SaveModel: sysBaseUrl + '/Develop/SaveModel',
        DeleteModel: sysBaseUrl + '/Develop/DeleteModel'
    };
    var _this = this;

    var toolbar = [
        {
            text: '新建', icon: 'fa fa-plus', handler: function () {
                form.clear();
                grid.setData([]);
                $('pre').html('');
            }
        },
        {
            text: '保存', icon: 'fa fa-save', style: 'primary', handler: function () {
                form.save(url.SaveModel, function (d) {
                    tree.reload();
                    _this.setModel(d.Id);
                });
            }
        },
        {
            text: '删除', icon: 'fa fa-trash-o', style: 'danger', handler: function () {
                Layer.confirm('确定要删除该模型吗？', function () {
                    Ajax.post(url.DeleteModel, {
                        id: form.Id.getValue()
                    }, function (data) {
                        tree.reload();
                    });
                });
            }
        },
        {
            text: '导出', icon: 'fa fa-sign-out', handler: function () {
                var gm = new GridManager(grid, null, []);
                gm.export();
            }
        },
        {
            text: '复制', icon: 'fa fa-copy', handler: function () {
                var data = grid.getData();
                if (data.length === 0) {
                    Layer.tips('暂无数据可复制！');
                    return;
                }

                var lines = [];
                var columns = grid.columns;
                for (var i = 0; i < data.length; i++) {
                    var row = [];
                    for (var j = 0; j < columns.length; j++) {
                        if (columns[j].export) {
                            var value = data[i][columns[j].field];
                            row.push(value);
                        }
                    }
                    lines.push(row.join('\t'));
                }

                var text = lines.join('\n');
                Utils.copy(text);
                Layer.tips('复制成功！');
            }
        }
    ]

    var form = new Form('Domain', {
        style: 'form3', labelWidth: 80,
        fields: [
            { field: 'Id', type: 'hidden' },
            { title: '系统', field: 'System', type: 'select', code: ['开发平台', '微型ERP平台', '水质监测平台', '水质检测平台', '实验室平台'], required: true },
            { title: '类别', field: 'Category', type: 'text', required: true },
            { title: '表前缀', field: 'Prefix', type: 'text', required: true },
            { title: '代码', field: 'Code', type: 'text', required: true },
            { title: '名称', field: 'Name', type: 'text', required: true },
            { title: '选项', field: 'chkPaste', type: 'checkbox', code: [{Code: '1', Name: '启用粘贴批量导入'}] }
        ],
        setData: function (e) {
            var fields = [];
            if (e.data.FieldData) {
                fields = JSON.parse(e.data.FieldData);
            }
            grid.setData(fields);
        },
        onSaving: function (d) {
            var fields = grid.getData();
            d.FieldData = JSON.stringify(fields);
        }
    });

    var grid = new Grid('Field', {
        edit: true, row: { Type: 'string', Length: 50 },
        columns: [
            { action: 'add', icon: 'fa fa-plus', align: 'center', width: '60px', aFormat: 'remove,up,down' },
            { title: '名称', field: 'Name', type: 'text', width: '100px', export: true },
            { title: '编码', field: 'Code', type: 'text', width: '100px', export: true },
            { title: '类型', field: 'Type', type: 'select', code: ['string', 'int', 'decimal', 'date'], width: '80px', export: true },
            { title: '长度', field: 'Length', type: 'text', width: '50px', export: true },
            { title: '必填', field: 'Required', type: 'checkbox', width: '40px', align: 'center', export: true },
            //{ title: '宽度', field: 'Width', type: 'text', width: '80px', export: true },
            //{ title: '对齐', field: 'Align', type: 'select', code: ['center', 'right'], width: '100px', align: 'center', export: true },
            { title: '查询', field: 'Query', type: 'checkbox', width: '40px', align: 'center', export: true },
            //{ title: '排序', field: 'Sort', type: 'checkbox', width: '40px', align: 'center', export: true },
            //{ title: '仅表单', field: 'OnlyForm', type: 'checkbox', width: '50px', align: 'center', export: true },
            { title: '控件', field: 'Control', type: 'select', code: ['hidden', 'text', 'textarea', 'radio', 'checkbox', 'select', 'date', 'editor', 'html', 'picker'], width: '100px', export: true },
            { title: '代码表', field: 'Codes', type: 'text', export: true },
            //{ title: '导入', field: 'Import', type: 'checkbox', width: '40px', align: 'center', export: true },
            //{ title: '导出', field: 'Export', type: 'checkbox', width: '40px', align: 'center', export: true }
        ]
    });

    //methods
    this.render = function () {
        var elem = $('<div>').addClass('fit');
        var tb = $('<div>').css({ padding: '5px' }).appendTo(elem);
        _initToolbar(tb);
        form.render().appendTo(elem);
        grid.render().css({ top: '120px', overflow: 'auto' }).appendTo(elem);
        return elem;
    }

    this.setModel = function (id) {
        Ajax.get(url.GetModel, { id: id }, function (data) {
            form.setData(data.Model);
            for (var p in data) {
                var elem = $('#code' + p).addClass('prettyprint source linenums');
                if (elem.length) {
                    elem.html('<code>' + data[p] + '</code>');
                }
            }
            prettyPrint();
        });
    }

    this.bindEnablePaste = function () {
        $('[name="chkPaste"]').change(function () {
            _enablePaste($(this)[0].checked);
        });
    }

    //private
    function _initToolbar(container) {
        for (var i = 0; i < toolbar.length; i++) {
            var item = toolbar[i];
            Utils.createButton(item).appendTo(container);
        }
    }

    function _enablePaste(enabled) {
        Utils.paste(enabled, function (text) {
            var rows = [], columns = grid.columns, lines = text.split('\n');
            for (var i = 0; i < lines.length; i++) {
                if (!lines[i].length)
                    continue;

                var row = {}, line = lines[i].split('\t');
                for (var j = 0; j < columns.length; j++) {
                    //if (line.length + 1 > j) {
                        var col = columns[j + 1];
                        if (col && col.field) {
                            row[col.field] = line[j];
                        }
                    //}
                }
                rows.push(row);
            }
            grid.setData(rows);
        });
    }
}

$.extend(Page, {
    DevCode: { component: new DevCode() }
});