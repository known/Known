//mini.parse();

var Navbar = {
    demo: function () {
        var tab = MainTabs.active({
            id: 'demo', iconCls: 'fa-puzzle-piece', text: '开发示例'
        });
        $(tab.bodyEl).loadHtml('/view/partial', {
            name: 'Demo/DemoView'
        }, function () {
            DemoView.show();
        });
    },
    todo: function () {
        MainTabs.active({
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

var MainTabs = {
    tabsId: 'mainTabs',
    active: function (item) {
        var tabs = mini.get(this.tabsId);
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
    },
    home: function () {
        this.active({ id: 'index' });
    },
    refresh: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        tabs.reloadTab(tab);
    },
    remove: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        if (tab.name !== 'index') {
            tabs.removeTab(tab);
        }
    },
    fullScreen: function () {
    }
};

$(function () {
    //menu
    var menu = new Menu('#mainMenu', {
        itemclick: function (item) {
            if (!item.children) {
                MainTabs.active(item);
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
    $('#navDemo').click(function () { Navbar.demo(); });
    $('#navTodo').click(function () { Navbar.todo(); });

    //userinfo menu
    $('#ddmUserInfo').click(function () { UserMenu.info(); });
    $('#ddmUpdatePwd').click(function () { UserMenu.updPwd(); });
    $('#ddmLogout').click(function () { UserMenu.logout(); });

    $('#tabsButtons .fa-home').click(function () { MainTabs.home(); });
    $('#tabsButtons .fa-refresh').click(function () { MainTabs.refresh(); });
    $('#tabsButtons .fa-remove').click(function () { MainTabs.remove(); });
    $('#tabsButtons .fa-arrows-alt').click(function () { MainTabs.fullScreen(); });
});