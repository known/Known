var ModuleForm = {

    show: function (option) {
        $.extend(true, this, FormBase);

        $.extend(option, {
            toolbarId: 'tbModuleForm',
            formId: 'formModule',
            formInit: function (f, d) {
                $('#moduleIcon').attr('class', 'mini-icon mini-iconfont ' + d.Icon);
            },
            saveUrl: '/api/plt/Module/SaveModule'
        });

        this.init(option);
        //this.form.Icon.on('drawcell', this.onIconDrawCell);
    },

    onIconDrawCell: function (e) {
        var item = e.record, field = e.field, value = e.value;
        e.cellHtml = '<span class="fa ' + value + '" style="width:16px;"> ' + value + '</span>';
    }

};