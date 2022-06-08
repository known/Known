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

function SysSetting() {
    var url = {
        GetModel: baseUrl + '/System/GetSetting',
        SaveModel: baseUrl + '/System/SaveSetting'
    };

    var form = new Form('SysSetting', {
        icon: 'fa fa-cog', title: Language.SystemSetting, info: true,
        fields: [
            { title: Language.CompNo, field: 'CompNo', type: 'text', readonly: true },
            { title: Language.Product + 'ID', field: 'ProductId', type: 'text', readonly: true },
            { title: Language.CompName, field: 'CompName', type: 'text', required: true, inputBlock: true },
            { title: Language.ProductSerialNo, field: 'ProductKey', type: 'text', required: true, inputBlock: true },
            { title: Language.SystemName, field: 'AppName', type: 'text', required: true, inputBlock: true },
            { title: Language.ProductValidDate, field: 'ValidDate', type: 'text', readonly: true, tips: '天' },
            //{
            //    title: '文件位置', field: 'UploadPath', type: 'text', lineBlock: true, inputStyle: 'width:50%',
            //    tips: '文件上传服务器的位置，建议不要放在系统盘。'
            //},
            //{ title: 'SMTP服务器', field: 'SmtpServer', type: 'text', tips: '填写IP或者域名。' },
            //{ title: 'SMTP端口', field: 'SmtpPort', align: 'right', type: 'text' },
            //{ title: '发件人名称', field: 'FromName', type: 'text' },
            //{ title: '发件人邮箱', field: 'FromEmail', type: 'text' },
            //{ title: '发件人密码', field: 'FromPassword', type: 'password' },
            { title: Language.UserDefaultPwd, field: 'UserDefaultPwd', type: 'text', required: true }
        ],
        toolbar: [{
            text: Language.Save, icon: 'fa fa-check', handler: function (e) {
                e.form.save(url.SaveModel);
            }
        }]
    });

    //methods
    this.render = function (dom) {
        form.render().css({ display: 'block' }).appendTo(dom);
    }

    this.mounted = function () {
        form.ValidDate.setTips('&nbsp;');
        $.get(url.GetModel, function (res) {
            form.setData(res);
            var validDate = res.ValidDate;
            if (validDate !== '永久') {
                var date = Date.parse(validDate);
                var now = new Date();
                var diff = parseInt((date - now) / (1000 * 60 * 60 * 24)) + 1;
                var tip = Utils.format(Language.ProductValidDateTip, diff);
                form.ValidDate.setTips(tip, { color: '#e00211' });
            }
        });
    }
}

$.extend(Page, {
    SysSetting: { component: new SysSetting() }
});