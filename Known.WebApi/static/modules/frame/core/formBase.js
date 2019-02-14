var FormBase = {

    option: {
        saveUrl: '/api/plt/Prototype/SaveForm'
    },

    form: null,

    init: function (option) {
        this.option = option;
        new Toolbar(option.toolbarId, this);
        this.form = new Form(option.formId, {
            data: option.data,
            callback: option.formInit
        });
    },

    save: function () {
        var _this = this;
        this.form.saveData({
            url: _this.option.saveUrl,
            callback: function (data) {
                _this.form.setData(data, _this.option.callback);
            }
        });
    },

    close: function () {
        Dialog.close();
    }

};