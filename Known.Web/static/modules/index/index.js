//mini.parse();

function activeTab(item) {
    var tabs = mini.get('mainTabs');
    var tab = tabs.getTab(item.id);
    if (!tab) {
        tab = tabs.addTab({
            name: item.id, title: item.text, url: item.url,
            iconCls: item.iconCls, showCloseButton: true
        });
    }
    tabs.activeTab(tab);
    tab.bodyEl = tabs.getTabBodyEl(tab);
    return tab;
}

var Navbar = {
    demo: function () {
        var tab = activeTab({
            id: 'demo', iconCls: 'fa-puzzle-piece', text: '开发示例'
        });
        $(tab.bodyEl).loadHtml('/view/partial', {
            name: 'Demo/DemoView'
        }, function () {
            DemoView.show();
        });
    },
    todo: function () {
        activeTab({
            id: 'todo', iconCls: 'fa-paper-plane',
            text: '代办事项', url: '/home/todo'
        });
    }
};

var UserMenu = {
    info: function () {
        Ajax.getJson('/api/user/getuserinfo', function (data) {
        });
    },
    updPwd: function () {
    },
    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/user/signout', function () {
                location = location;
            });
        });
    }
};

$(function () {
    //menu
    var menu = new Menu('#mainMenu', {
        itemclick: function (item) {
            if (!item.children) {
                activeTab(item);
            }
        }
    });

    $('.sidebar').mCustomScrollbar({ autoHideScrollbar: true });

    new MenuTip(menu);

    Ajax.getJson('/api/user/getmenus', function (result) {
        menu.loadData(result.Data);
    });

    //toggle
    $('#toggle, .sidebar-toggle').click(function () {
        $('body').toggleClass('compact');
        mini.layout();
    });

    //dropdown
    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });
    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });

    //navbar
    $('#navDemo').click(Navbar.demo);
    $('#navTodo').click(Navbar.todo);

    //userinfo menu
    $('#ddmUserInfo').click(UserMenu.info);
    $('#ddmUpdatePwd').click(UserMenu.updPwd);
    $('#ddmLogout').click(UserMenu.logout);

});