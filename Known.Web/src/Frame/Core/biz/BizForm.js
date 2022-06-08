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

var BizForm = {

    open: function (option) {
        var elem = $('<div>').addClass('form form-block');
        var width = option.width;
        if (!width) {
            width = Utils.checkMobile() ? 300 : 400;
        }

        Layer.open({
            title: option.title,
            width: width, height: option.height || 200,
            content: elem,
            success: function () {
                var form = new Form(elem, { fields: option.fields });
                if (option.data) {
                    form.setData(option.data);
                }
                option.success && option.success({ form: form });
            },
            buttons: option.buttons
        });
    },

    close: function (title, callback) {
        var form;
        this.open({
            title: title,
            fields: [{ title: Language.Close + Language.Reason, field: 'Reason', type: 'textarea', required: true }],
            success: function (e) { form = e.form; },
            buttons: [{
                text: Language.OK, handler: function (e) {
                    if (!form.validate())
                        return;

                    Layer.confirm(Language.OK + title + '？', function () {
                        e.close();
                        var data = form.getData();
                        callback && callback(data.Reason);
                    });
                }
            }]
        });
    }

}