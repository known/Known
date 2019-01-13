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

    Ajax.getJson('/api/plt/User/GetModules', function (result) {
        menu.loadData(result.menus);
        Code.setData(result.codes);
    });

    //toggle
    $('#toggle, .sidebar-toggle').click(function () {
        var body = $('body'), toggle = $('.sidebar-toggle i');
        body.toggleClass('compact');
        if (body.hasClass('compact')) {
            toggle.removeClass('fa-dedent').addClass('fa-indent');
        } else {
            toggle.removeClass('fa-indent').addClass('fa-dedent');
        }
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

    //toolbars
    new Toolbar('tbNavbar', Navbar);
    new Toolbar('tbMainTabs', MainTabs);

    mini.parse();
    MainTabs.index();

});