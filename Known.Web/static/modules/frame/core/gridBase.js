var GridBase = {

    option: null,
    grid: null,

    init: function (option) {
        this.option = option;
        var code = option.moduleCode.toLowerCase();
        this.option.deleteUrl = '/' + code + '/delete' + code + 's';
        new Toolbar('tb' + option.moduleCode, this);
        this.grid = new Grid(option.moduleCode);
    },

    showForm: function (data) {
        var _this = this, option = this.option;
        Dialog.show({
            name: option.formView,
            title: option.moduleName + '【' + (data.Id === '' ? '新增' : '编辑') + '】',
            width: 500, height: 250,
            callback: function () {
                option.form.show({
                    data: data,
                    callback: function () {
                        _this.grid.reload();
                        option.callback && option.callback();
                    }
                });
            }
        });
    },

    add: function () {
        this.showForm(this.option.formData);
    },

    edit: function () {
        var _this = this;
        this.grid.checkSelect(function (row) {
            _this.showForm(row);
        });
    },

    remove: function () {
        var _this = this;
        this.grid.deleteRows(function (rows) {
            var data = mini.encode(_this.getRowDatas(rows));
            Ajax.postJson(_this.option.deleteUrl, data, function (res) {
                Message.result(res, function () {
                    _this.grid.reload();
                    option.callback && option.callback();
                });
            });
        });
    },

    close: function () {
        window.CloseOwnerWindow();
    },

    getRowDatas: function (rows, fields) {
        var datas = [];
        if (fields) {
            $(rows).each(function (i, d) {
                var data = {};
                $(fields).each(function (i, p) {
                    data[p] = d[p] || '';
                });
                datas.push(data);
            });
        } else {
            var id = this.grid.idField;
            $(rows).each(function (i, d) { datas.push(d[id] || ''); });
        }
        return datas;
    }

};