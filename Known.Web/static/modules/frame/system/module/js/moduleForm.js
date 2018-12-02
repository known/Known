var ModuleForm = {

    option: null,
    toolbar: null,
    form: null,

    show: function (option) {
        this.option = option;
        this.toolbar = new Toolbar('tbModuleForm', this);
        this.form = new Form('formModule', {
            data: option.data,
            callback: function (f, d) {
                $('#moduleIcon').attr('class', 'mini-icon mini-iconfont ' + d.Icon);
            }
        });
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