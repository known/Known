function activeTab(item) {
    var tabs = mini.get("mainTabs");
    var tab = tabs.getTab(item.id);
    if (!tab) {
        tab = { name: item.id, title: item.text, url: item.url, iconCls: item.iconCls, showCloseButton: true };
        tab = tabs.addTab(tab);
    }
    tabs.activeTab(tab);
}

$(function () {

    //menu
    var menu = new Menu("#mainMenu", {
        itemclick: function (item) {
            if (!item.children) {
                activeTab(item);
            }
        }
    });

    $(".sidebar").mCustomScrollbar({ autoHideScrollbar: true });

    new MenuTip(menu);

    $.ajax({
        url: "/menu.json",
        success: function (text) {
            var data = mini.decode(text);
            menu.loadData(data);
        }
    })

    //toggle
    $("#toggle, .sidebar-toggle").click(function () {
        $('body').toggleClass('compact');
        mini.layout();
    });

    //dropdown
    $(".dropdown-toggle").click(function (event) {
        $(this).parent().addClass("open");
        return false;
    });

    $(document).click(function (event) {
        $(".dropdown").removeClass("open");
    });
});