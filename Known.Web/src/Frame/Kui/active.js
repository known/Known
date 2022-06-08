function Install() {
    //fields
    var url = {
        GetInstallInfo: baseUrl + '/Home/Install?data=1',
        SaveInstall: baseUrl + '/Home/Install'
    };
    var box = new InitBox({
        getUrl: url.GetInstallInfo, saveUrl: url.SaveInstall,
        style: 'install', title: '安装程序', btnText: '安 装',
        fields: [
            { title: '企业编码', field: 'CompNo', type: 'text', required: true },
            { title: '企业名称', field: 'CompName', type: 'text', required: true },
            { title: '系统名称', field: 'AppName', type: 'text', required: true },
            { title: '产品ID', field: 'ProductId', type: 'text', readonly: true },
            { title: '产品密钥', field: 'ProductKey', type: 'text', required: true },
            { title: '管理员账号', field: 'UserName', type: 'text', readonly: true },
            { title: '管理员密码', field: 'Password', type: 'password', required: true },
            { title: '确认密码', field: 'Password1', type: 'password', required: true }
        ]
    });

    //methods
    this.render = function (dom) {
        box.render(dom);
    }

    this.mounted = function () {
        box.mounted();
    }
}

function Active() {
    //fields
    var url = {
        GetActiveInfo: baseUrl + '/Home/Active?data=1',
        SaveActive: baseUrl + '/Home/Active'
    };
    var box = new InitBox({
        getUrl: url.GetActiveInfo, saveUrl: url.SaveActive,
        title: Language.ActiveTitle, btnText: Language.Active,
        fields: [
            { title: Language.CompNo, field: 'CompNo', type: 'text', readonly: true },
            { title: Language.CompName, field: 'CompName', type: 'text', readonly: true },
            { title: Language.ProductId, field: 'ProductId', type: 'text', readonly: true },
            { title: Language.SerialNo, field: 'ProductKey', type: 'text', readonly: true }
        ]
    });

    //methods
    this.render = function (dom) {
        box.render(dom);
    }

    this.mounted = function () {
        box.mounted();
    }
}

function InitBox(option) {
    var form = new Form('InitBox', {
        style: 'form-block', fields: option.fields,
        toolbar: [{
            text: option.btnText, handler: function (e) {
                if (!e.form.validate())
                    return;

                var data = e.form.getData();
                var btn = $(this).attr('disabled', true);
                $.post(option.saveUrl, {
                    data: JSON.stringify(data)
                }, function(res) {
                    if (!res.IsValid) {
                        _message(res.Message, true);
                        btn.attr('disabled', false);
                    } else {
                        _message(res.Message, false);
                        app.login();
                    }
                });
            }
        }]
    });

    //methods
    this.render = function (dom) {
        _createHeader(dom);
        _createBody(dom);
        _createFooter(dom);
    }

    this.mounted = function () {
        $.post(option.getUrl, function (res) {
            form.setData(res);
            if (res.Message) {
                box.message(res.Message, true);
            }
        });
    }

    this.message = function (text, error) {
        _message(text, error);
    }

    //private
    function _createHeader(dom) {
        var header = $('<div>').addClass('active-header').appendTo(dom);
        $('<h1>').html($('title').text()).appendTo(header);
    }

    function _createBody(dom) {
        var body = $('<div>').addClass('active-body').appendTo(dom);
        var box = $('<div>').addClass('active-box').appendTo(body);
        if (option.style) {
            box.addClass(option.style);
        }
        $('<h1>').addClass('active-title').html(option.title).appendTo(box);

        form.render().appendTo(box);
        $('<div>').addClass('message').appendTo(box);
    }

    function _createFooter(dom) {
        $('<div>').addClass('active-footer')
            .append(copyrightInfo)
            .appendTo(dom);
    }

    function _message(text, error) {
        var div = $('.message').html('');
        if (error) {
            $('<span>').addClass('red').html(text).appendTo(div);
        } else {
            div.html(text);
        }
    }
}