mini.parse();

function activeTab(item) {
    var tabs = mini.get('mainTabs');
    var tab = tabs.getTab(item.id);
    if (!tab) {
        tab = { name: item.id, title: item.text, url: item.url, iconCls: item.iconCls, showCloseButton: true };
        tab = tabs.addTab(tab);
    }
    tabs.activeTab(tab);
}

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
    $('#navTodo').click(function () {
        activeTab({ id: 'todo', iconCls: 'fa-paper-plane', text: '代办事项' });
    });

    //userinfo menu
    $('#ddmUserInfo').click(function () {
        Ajax.getJson('/api/user/getuserinfo', function (data) {
        });
    });
    $('#ddmUpdatePwd').click(function () {
    });
    $('#ddmLogout').click(function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/user/signout', function () {
                location = location;
            });
        });
    });

});