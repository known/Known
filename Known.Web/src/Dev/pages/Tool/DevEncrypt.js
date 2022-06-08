function DevEncrypt() {
    var tabs = new Tabs({
        fit: true,
        items: [
            { name: '加解密', component: new DevEncryptString() },
            { name: '序列号', component: new DevEncryptSerialNo() },
            { name: '平台API加密', component: new DevEncryptApiKey() }
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

function DevEncryptApiKey() {
    var url = {
        GenerateApiKey: sysBaseUrl + '/Develop/GenerateApiKey'
    };

    var form = new Form('ApiKey', {
        style: 'form-block',
        fields: [
            { title: 'ApiUrl', field: 'Url', type: 'text', inputStyle: 'width:280px', required: true },
            { title: 'ApiToken', field: 'Token', type: 'text', inputStyle: 'width:280px', required: true, readonly: true },
            { title: 'Key', field: 'Key', type: 'textarea', readonly: true }
        ],
        toolbar: [
            { text: '加密', icon: 'fa fa-lock', handler: function (e) { genApiKey(e.form); } },
            { text: '下载', icon: 'fa fa-download', handler: function (e) { downloadApiKey(e.form); } }
        ]
    });

    //methods
    this.render = function () {
        return form.render();
    }

    this.mounted = function () {
        form.setData({
            Url: 'http://api.pumantech.com',
            Token: '0414a351af414f70918d9217811be8de'
        });
    }

    //private
    function genApiKey(form) {
        var data = form.getData();
        Ajax.post(url.GenerateApiKey, {
            data: JSON.stringify(data)
        }, function (res) {
            form.Key.setValue(res);
        });
    }

    function downloadApiKey(form) {
        Utils.genFile(form.Key.getValue(), 'License.key');
    }
}

$.extend(Page, {
    DevEncrypt: { component: new DevEncrypt() }
});