var ModuleForm = {

    show: function (option) {
        $.extend(true, this, FormBase);

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
    }

};