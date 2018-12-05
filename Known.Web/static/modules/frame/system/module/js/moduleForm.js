var ModuleForm = {

    option: null,
    form: null,

    show: function (option) {
        this.option = option;
        new Toolbar('tbModuleForm', this);

        this.form = new Form('formModule', {
            data: option.data,
            callback: function (f, d) {
                $('#moduleIcon').attr('class', 'mini-icon mini-iconfont ' + d.Icon);
            }
        });
        //this.form.Icon.on('drawcell', this.onIconDrawCell);
    },

    onIconDrawCell: function (e) {
        var item = e.record, field = e.field, value = e.value;
        e.cellHtml = '<span class="fa ' + value + '" style="width:16px;"> ' + value + '</span>';
    },

    save: function () {
        var _this = this;
        this.form.saveData({
            url: '/api/module/savemodule',
            callback: function (data) {
                _this.form.setData(data, _this.option.callback);
            }
        });
    },

    close: function () {
        Dialog.close();
    }
};