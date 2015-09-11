$(function () {
    $('#side-menu').metisMenu();
});

//Loads the correct sidebar on window load,
//collapses the sidebar on window resize.
// Sets the min-height of #page-wrapper to window size
$(function () {
    $(window).bind("load resize", function () {
        topOffset = 50;
        width = (this.window.innerWidth > 0) ? this.window.innerWidth : this.screen.width;
        if (width < 768) {
            $('div.navbar-collapse').addClass('collapse');
            topOffset = 100; // 2-row-menu
        } else {
            $('div.navbar-collapse').removeClass('collapse');
        }

        height = ((this.window.innerHeight > 0) ? this.window.innerHeight : this.screen.height) - 1;
        height = height - topOffset;
        if (height < 1) height = 1;
        if (height > topOffset) {
            $("#page-wrapper").css("min-height", (height) + "px");
        }
    });

    var url = window.location;
    var element = $('ul.nav a').filter(function () {
        return this.href == url || url.href.indexOf(this.href) == 0;
    }).addClass('active').parent().parent().addClass('in').parent();
    if (element.is('li')) {
        element.addClass('active');
    }
});

function page(id) {
    var parentText = $('#' + id).parent().parent().prev().text().substring(2);
    var breadcrumb = parentText + '&nbsp;>&nbsp;' + $('#' + id).text();
    $('#breadcrumb').html(breadcrumb);
    $('#content').html('Loading......');
    $('#content').load(virtualPath + 'ajax.ashx?act=load_page', { Id: id }, function () { });
}

function dialog(selector) {
    $(selector + ' :hidden').val('');
    $(selector + ' :text').val('');
    $(selector + ' :checkbox').attr('checked', false);
    $(selector + ' textarea').val('');
    $(selector).modal('show');
}

var Ajax = {
    get: function (act, data, callback) {
        var url = virtualPath + 'ajax.ashx?act=' + act;
        $.get(url, data, function (result) {
            Ajax.handle(result, callback);
        });
    },
    post: function (act, data, callback) {
        var url = virtualPath + 'ajax.ashx?act=' + act;
        $.post(url, data, function (result) {
            Ajax.handle(result, callback);
        });
    },
    handle: function (result, callback) {
        if (result && result.length) {
            var obj = $.parseJSON(result);
            if (obj && callback) {
                callback(obj);
            }
        } else {
            alert('无效的请求！');
        }
    }
};

var Model = {
    get: function (keys) {
        var model = {};
        $(keys.split(',')).each(function (i, k) {
            model[k] = $('#' + k).val();
        });
        return model;
    },
    set: function (model, keys) {
        $(keys.split(',')).each(function (i, k) {
            $('#' + k).val(model ? model[k] : '');
        });
    },
    getChecked: function (selector) {
        var values = [];
        $(selector + ' :checked').each(function (i, m) {
            values.push($(m).val());
        });
        return values.join(',');
    },
    setChecked: function (selector, value) {
        var values = ',' + value + ',';
        $(selector + ' :checkbox').each(function (i, m) {
            var checked = values.indexOf(',' + $(m).val() + ',') > -1;
            $(m).attr('checked', checked);
        });
    }
};


//-----------------------------------------Setting----------------------------------------------
var Setting = {
    save: function (keys) {
        Ajax.post('save_setting', Model.get(keys), function (result) {
            alert(result.AlertMessage);
        });
        return false;
    }
};

//-----------------------------------------Menu-------------------------------------------------
var Menu = {
    save: function () {
        var data = Model.get('Id,Parent,Code,Name,Icon,Url');
        Ajax.post('save_menu', data, function (result) {
            alert(result.AlertMessage);
        });
        return false;
    },
    edit: function (id) {
        Ajax.get('get_menu', { Id: id }, function (result) {
            Model.set(result, 'Id,Parent,Code,Name,Icon,Url');
            $('#menuModal').modal('show');
        });
    },
    remove: function (id) {
        if (confirm('确定删除吗？')) {
            Ajax.post('remove_menu', { Id: id }, function (result) {
                alert(result.AlertMessage);
            });
        }
    },
    addSub: function (id, name) {
        $('#Parent').val(id);
        $('#menuTitle').html('添加子菜单 - ' + name);
        Model.set(null, 'Id,Code,Name,Icon,Url');
        $('#menuModal').modal('show');
    },
    up: function (id) {
        Ajax.post('up_menu', { Id: id }, function (result) {
            alert(result.AlertMessage);
        });
    },
    down: function (id) {
        Ajax.post('down_menu', { Id: id }, function (result) {
            alert(result.AlertMessage);
        });
    },
    toggle: function (id) {
        if ($('#' + id).attr('aria-expanded') == '0') {
            $('#' + id).attr('aria-expanded', '1');
            $('#' + id).removeClass('glyphicon-chevron-right').addClass('glyphicon-chevron-down');
            $('tr[parent="' + id + '"]').removeClass('hide');
        } else {
            $('#' + id).attr('aria-expanded', '0');
            $('#' + id).removeClass('glyphicon-chevron-down').addClass('glyphicon-chevron-right');
            $('tr[parent="' + id + '"]').addClass('hide');
        }
    }
};

//-----------------------------------------RoleManage-------------------------------------------
var Role = {
    save: function () {
        var data = Model.get('Id,Name,Description');
        Ajax.post('save_role', data, function (result) {
            alert(result.AlertMessage);
            if (result.IsSuccess) {
                $('#roleModal').modal('hide');
            }
        });
        return false;
    },
    edit: function (id) {
        Ajax.get('get_role', { Id: id }, function (result) {
            Model.set(result, 'Id,Name,Description');
            $('#roleModal').modal('show');
        });
    },
    remove: function (id) {
        if (confirm('确定删除吗？')) {
            Ajax.post('remove_role', { Id: id }, function (result) {
                alert(result.AlertMessage);
            });
        }
    },
    saveRight: function () {
        var data = Model.get('RoleId');
        data.Menus = Model.getChecked('#menus');
        Ajax.post('save_right', data, function (result) {
            alert(result.AlertMessage);
        });
        return false;
    },
    setRight: function (id) {
        Ajax.get('get_right', { Id: id }, function (result) {
            $('#RoleId').val(result.Role.Id);
            $('#RoleName').html(result.Role.Name);
            var menus = ',' + result.Role.Menus + ',';
            var html = '<table class="table table-hover"><thead><tr><th style="width:1px;"></th><th style="width:60px;">选择</th><th>菜单编码</th><th>菜单名称</th><th>菜单地址</th></tr></thead><tbody>';
            $(result.Menus).each(function (i, m) {
                var attachAttrs = !m.Parent ? ' class="info"' : ' parent="' + m.Parent + '" class="hide"',
                    toggle = !m.Parent ? '<i id="' + m.Id + '" class="glyphicon glyphicon-chevron-right" onclick="Menu.toggle(\'' + m.Id + '\');" aria-expanded="0"></i>' : ''
                    checked = menus.indexOf(',' + m.Id + ',') > -1 ? ' checked' : '';
                html += '<tr' + attachAttrs + '><td>' + toggle + '</td><td><input type="checkbox" name="Menus" value="' + m.Id + '"' + checked + '></td><td>' + m.Code + '</td><td>' + m.Name + '</td><td>' + m.Url + '</td></tr>';
            });
            html += '</tbody></table>';
            $('#menus').html(html);
            $('#rightModal').modal('show');
        });
    }
};

//-----------------------------------------UserManage-------------------------------------------
var User = {
    save: function () {
        var data = Model.get('Id,UserName,DisplayName,Email');
        data.Roles = Model.getChecked('#roles');
        Ajax.post('save_user', data, function (result) {
            alert(result.AlertMessage);
        });
        return false;
    },
    edit: function (id) {
        Ajax.get('get_user', { Id: id }, function (result) {
            Model.set(result, 'Id,UserName,DisplayName,Email');
            Model.setChecked('#roles', result.Roles);
            $('#UserName').attr('disabled', true);
            $('#userModal').modal('show');
        });
    },
    remove: function (id) {
        if (confirm('确定删除吗？')) {
            Ajax.post('remove_user', { Id: id }, function (result) {
                alert(result.AlertMessage);
            });
        }
    },
    resetPwd: function (id) {
        if (confirm('确定重置密码吗？')) {
            Ajax.post('reset_password', { Id: id }, function (result) {
                alert(result.AlertMessage);
            });
        }
    },
    changePwd: function () {
        $('form').validate();
        var data = Model.get('CurrentUserId,OldPassword,NewPassword,NewPassword1');
        Ajax.post('change_password', data, function (result) {
            alert(result.AlertMessage);
        });
        return false;
    }
};