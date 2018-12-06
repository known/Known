var FormBase = {

    option: {},
    form: null,

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