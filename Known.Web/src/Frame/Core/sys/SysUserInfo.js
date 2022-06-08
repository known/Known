function SysUserInfo() {
    var tabs = new Tabs({
        fit: true,
        items: [
            { name: Language.BaseInfo, component: new SysUserBaseInfo() },
            { name: Language.UpdatePassword, component: new SysUserPassword() },
            { name: Language.MyMessage, component: new SysUserMessage() }
        ]
    });

    //methods
    this.render = function (dom) {
        var fit = $('<div>').addClass('fit').appendTo(dom);
        var left = $('<div>').addClass('fit-col-3').appendTo(fit);

        var info = $('<div>').css({ padding: '10px' }).appendTo(left);
        $('<div>').addClass('center')
            .css({ padding: '20px' })
            .append('<img id="userAvatar" class="circle" src="' + staticUrl + '/img/face1.png" />')
            .appendTo(info);
        $('<div>').addClass('info-item')
            .append('<i class="fa fa-user">')
            .append('<span id="userName">')
            .appendTo(info);
        $('<div>').addClass('info-item')
            .append('<i class="fa fa-mobile" style="font-size:16px;">')
            .append('<span id="userMobile">')
            .appendTo(info);
        $('<div>').addClass('info-item')
            .append('<i class="fa fa-envelope-o" style="font-size:11px;">')
            .append('<span id="userEmail">')
            .appendTo(info);
        $('<div id="userNote">').addClass('info-item').appendTo(info);

        var right = $('<div>').addClass('fit-col-7').appendTo(fit);
        tabs.render().appendTo(right);
    }

    this.mounted = function () {
    }

    //private
}

function SysUserBaseInfo() {
    var url = {
        GetUserInfo: baseUrl + '/Home/GetUserInfo',
        SaveUserInfo: baseUrl + '/Home/SaveUserInfo'
    };

    var form = new Form('UserInfo', {
        url: url.GetUserInfo,
        fields: [
            { field: 'Id', type: 'hidden' },
            { field: 'AvatarUrl', type: 'hidden' },
            { title: Language.UserName, field: 'UserName', type: 'text', readonly: true },
            { title: Language.UserRealName, field: 'Name', type: 'text', required: true },
            { title: Language.EnglishName, field: 'EnglishName', type: 'text' },
            { title: Language.Gender, field: 'Gender', type: 'radio', code: [Language.Male, Language.Female] },
            { title: Language.Phone, field: 'Phone', type: 'text' },
            { title: Language.Mobile, field: 'Mobile', type: 'text' },
            { title: Language.Email, field: 'Email', type: 'text' },
            { title: Language.Note, field: 'Note', type: 'textarea', lineBlock: true }
        ],
        setData: function (e) {
            $('#userAvatar').attr('src', staticUrl + e.data.AvatarUrl);
            $('#userName').html(e.data.Name + '(' + e.data.UserName + ')');
            $('#userMobile').html(e.data.Mobile);
            $('#userEmail').html(e.data.Email);
            $('#userNote').html(e.data.Note);
        },
        onSaving: function (data) {
            var face = data.Gender === Language.Female ? '2' : '1';
            data.AvatarUrl = '/img/face' + face + '.png';
        },
        toolbar: [{
            text: Language.ConfirmUpdate, icon: 'fa fa-check', handler: function (e) {
                e.form.save(url.SaveUserInfo);
            }
        }]
    });

    //methods
    this.render = function () {
        return form.render();
    }
}

function SysUserPassword() {
    var url = {
        UpdatePassword: baseUrl + '/Home/UpdatePassword'
    };

    var form = new Form('Password', {
        style: 'form-block',
        fields: [
            { title: Language.CurrentPassword, field: 'OldPassword', type: 'password', required: true },
            { title: Language.NewPassword, field: 'NewPassword', type: 'password', required: true, tips: Language.PasswordTips },
            { title: Language.NewPassword1, field: 'NewPassword1', type: 'password', required: true }
        ],
        toolbar: [{
            text: Language.ConfirmUpdate, icon: 'fa fa-check', handler: function (e) {
                e.form.save(url.UpdatePassword);
            }
        }]
    });

    //methods
    this.render = function () {
        return form.render();
    }
}

function SysUserMessage() {
    //fields
    var url = {
        QueryUserMessages: baseUrl + '/System/QueryUserMessages',
        DeleteUserMessages: baseUrl + '/System/DeleteUserMessages',
        MarkUserMessages: baseUrl + '/System/MarkUserMessages'
    };
    var _right;
    var _leftButtons = [
        { text: '收件箱', icon: 'fa fa-sign-in', handler: function () { _showGrid('收件'); } },
        { text: '已发送', icon: 'fa fa-sign-out', handler: function () { _showGrid('发件') } },
        { text: '已删除', icon: 'fa fa-trash-o', handler: function () { _showGrid('删除') } }
    ];

    //methods
    this.render = function () {
        var elem = $('<div>').addClass('fit');
        var left = $('<div>').addClass('um-box-left').appendTo(elem);
        _createButtons(left, _leftButtons);

        _right = $('<div>').addClass('um-box-right').appendTo(elem);
        return elem;
    }

    this.mounted = function () {
        $('.um-box-button:eq(0)').click();
    }

    //private
    function _createButtons(container, buttons) {
        for (var i = 0; i < buttons.length; i++) {
            var btn = buttons[i];
            $('<div>').addClass('um-box-button')
                .data('button', btn)
                .append('<i class="' + btn.icon + '">')
                .append('<span>' + btn.text + '</span>')
                .on('click', function () {
                    $('.um-box-button').removeClass('active');
                    $(this).addClass('active');
                    var item = $(this).data('button');
                    item.handler();
                })
                .appendTo(container);
        }
    }

    function _showGrid(type) {
        var option = _getMessageOption(type);
        var grid = new Grid('UserMessage', {
            url: url.QueryUserMessages, where: { Type: type }, autoQuery: false,
            toolButtons: option.toolButtons,
            columns: [
                { title: option.rsTitle, field: 'MsgByName', width: '150px' },
                { title: '分类', field: 'Category', width: '100px' },
                { title: '主题', field: 'Subject' },
                { title: '时间', field: 'CreateTime', width: '140px', placeholder: DateTimeFormat }
            ]
        });

        _right.html('');
        grid.render().appendTo(_right);
        Page.complete();
        setTimeout(function () { grid.reload(); }, 100);
    }

    function _getMessageOption(type) {
        var title = '';
        var buttons = [];
        if (type === '收件') {
            title = '发件人';
            buttons.push({ name: '删除', icon: 'fa fa-trash-o', handler: function (e) { _deleteAction(e); } });
            buttons.push({
                name: '标记为', icon: 'fa fa-bookmark-o', children: [
                    { name: '已读', icon: 'fa fa-envelope-open-o', handler: function (e) { _markStatus(e, '已读'); } },
                    { name: '未读', icon: 'fa fa-envelope-o', handler: function (e) { _markStatus(e, '未读'); } },
                    { name: '全部设为已读', icon: 'fa fa-envelope-open-o', handler: function (e) { _markStatus(e, '全部已读'); } }
                ]
            })
        } else if (type === '发件') {
            title = '收件人';
            buttons.push({ name: '删除', icon: 'fa fa-trash-o', handler: function (e) { _deleteAction(e); } });
        } else if (type === '删除') {
            title = '收发件人';
            buttons.push({ name: '彻底删除', icon: 'fa fa-trash-o', handler: function (e) { _deleteAction(e); } });
        }

        return { rsTitle: title, toolButtons: buttons };
    }

    function _deleteAction(e) {
        e.deleteRows(url.DeleteUserMessages, function () {
            e.grid.reload();
        });
    }

    function _markStatus(e, status) {
        if (status === '全部已读') {
            Layer.confirm('确定全部设为已读？', function () {
                Ajax.post(url.MarkUserMessages, {
                    status: status, data: JSON.stringify(['1'])
                }, function () {
                    e.grid.reload();
                });
            });
        } else {
            e.selectRows(function (g) {
                Ajax.post(url.MarkUserMessages, {
                    status: status, data: JSON.stringify(g.ids)
                }, function () {
                    e.grid.reload();
                });
            });
        }
    }
}

$.extend(Page, {
    SysUserInfo: { component: new SysUserInfo() }
});