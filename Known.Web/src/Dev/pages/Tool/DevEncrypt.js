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

function DevEncrypt() {
    var tabs = new Tabs({
        fit: true,
        items: [
            { name: '加解密', component: new DevEncryptString() },
            { name: '序列号', component: new DevEncryptSerialNo() }
        ]
    });

    //methods
    this.render = function (dom) {
        var elem = $('<div>').addClass('card-fit').appendTo(dom);
        tabs.render().appendTo(elem);
    }
}

function DevEncryptString() {
    var url = {
        EncryptData: sysBaseUrl + '/Develop/EncryptData'
    };

    var form = new Form('Encrypt', {
        style: 'form-block',
        fields: [
            { title: '密码', field: 'Password', type: 'text', required: true },
            { title: '明文', field: 'Plaintext', type: 'textarea' },
            { title: '密文', field: 'Ciphertext', type: 'textarea' }
        ],
        toolbar: [
            { text: '加密', icon: 'fa fa-lock', handler: function (e) { encryptData('加密', e.form); } },
            { text: '解密', icon: 'fa fa-unlock', handler: function (e) { encryptData('解密', e.form); } }
        ]
    });

    //methods
    this.render = function () {
        return form.render();
    }

    //private
    function encryptData(type, form) {
        if (!form.validate())
            return;

        var password = form.Password.getValue();
        var text = type === '加密' ? form.Plaintext.getValue() : form.Ciphertext.getValue();
        Ajax.post(url.EncryptData, {
            type: type, password: password, text: text
        }, function (res) {
            if (type === '加密') {
                form.Ciphertext.setValue(res);
            } else {
                form.Plaintext.setValue(res);
            }
        });
    }
}

function DevEncryptSerialNo() {
    var url = {
        GenerateSerialNo: sysBaseUrl + '/Develop/GenerateSerialNo'
    };

    var form = new Form('SerialNo', {
        fields: [
            {
                title: 'GUID数量', field: 'GuidNum', type: 'text',
                inputHtml: '<span style="margin-left:10px;"><button type="button" id="btnGuid">生成</button></span>'
            },
            {
                title: '序列号数量', field: 'SnNum', type: 'text',
                inputHtml: '<span style="margin-left:10px;"><button type="button" id="btnSerialNo">生成</button></span>'
            },
            { title: 'GUID', field: 'GuidText', type: 'textarea', inputStyle: 'height:150px;' },
            { title: '序列号', field: 'SnText', type: 'textarea', inputStyle: 'height:150px;' }
        ]
    });

    //methods
    this.render = function () {
        return form.render();
    }

    this.mounted = function () {
        form.setData({ GuidNum: 1, SnNum: 1 });
        $('#btnGuid').click(function () {
            genSerialNo('GUID', form.GuidNum.getValue(), form.GuidText);
        });
        $('#btnSerialNo').click(function () {
            genSerialNo('SN', form.SnNum.getValue(), form.SnText);
        });
    }

    //private
    function genSerialNo(type, num, field) {
        num = num || 1;
        Ajax.post(url.GenerateSerialNo, {
            type: type, num: num
        }, function (res) {
            field.setValue(res);
        });
    }
}

$.extend(Page, {
    DevEncrypt: { component: new DevEncrypt() }
});