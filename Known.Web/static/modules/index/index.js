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

    mini.parse();
    MainTabs.index();
});