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

function SysChangePwd(data) {
    var url = {
        UpdatePassword: baseUrl + '/Home/UpdatePassword'
    };
    var _form;

    var fields = [
        { title: Language.CurrentPassword, field: 'OldPassword', type: 'password', required: true },
        { title: Language.NewPassword, field: 'NewPassword', type: 'password', required: true },
        { title: Language.NewPassword1, field: 'NewPassword1', type: 'password', required: true }
    ];

    var toolbar = [{
        icon: 'fa fa-save', text: Language.ConfirmUpdate, handler: function (e) {
            _form.save(url.UpdatePassword, function () {
                app.router.back();
            });
        }
    }];

    //methods
    this.render = function () {
        var elem = $('<div>').addClass('content').css({ padding: '20px' });
        var form = $('<div>').addClass('form').appendTo(elem);
        _form = new Form(form, { fields: fields, toolbar: toolbar });
        return elem;
    }
}