var ModuleGrid = {

    show: function (option) {
        $.extend(true, this, GridBase);

        $.extend(option, {
            moduleName: '模块管理',
            toolbarId: 'tbModule',
            gridName: 'Module',
            form: ModuleForm,
            formView: 'System/Module/ModuleForm',
            formData: {
                Id: '',
                ParentId: option.pid,
                Enabled: 'Y'
            }
        });

        this.init(option);

        var _this = this;
        this.grid.query.pid.setValue(option.pid);
        this.grid.load(function (e) {
            _this.option.formData.Sort = e.result.total + 1;
        });
    },

    //toolbar
    copy: function () {
        var _this = this;
        this.grid.checkSelect(function (row) {
            var data = mini.clone(row);
            data.Id = '';
            data.Sort = _this.grid.getData().length + 1;
            _this.showForm(data);
        });
    }

};