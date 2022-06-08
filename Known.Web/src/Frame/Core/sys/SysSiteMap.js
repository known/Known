function SysSiteMap(isPage) {
    //fields

    //methods
    this.render = function (dom) {
        var elem = isPage ? $('<div>').addClass('fit').css('padding-top', '10px').appendTo(dom) : dom;
        var menu = top.Admin.CurrentMenu;
        if (menu && menu.children && menu.children.length) {
            for (var i = 0; i < menu.children.length; i++) {
                var item = menu.children[i];
                _createMenu(elem, item);
            }
        }
    }

    this.mounted = function () {
    }

    //private
    function _createMenu(dom, menu) {
        var row = $('<div>').addClass('row dash-sitemap').appendTo(dom);
        var card = $('<div>').addClass('card').appendTo(row);
        _createMenuItem('<span>', card, menu).addClass('dash-menu');

        if (menu.children && menu.children.length > 0) {
            var div = $('<ul>').addClass('menu-items').appendTo(card);
            for (var i = 0; i < menu.children.length; i++) {
                var item = menu.children[i];
                _createMenuItem('<li>', div, item).click(function () {
                    var d = $(this).data('item');
                    showTabPage(d);
                });
            }
        }
    }

    function _createMenuItem(tagName, div, item) {
        var span = $(tagName).data('item', item).appendTo(div);
        var icon = $('<span>').addClass('icon').appendTo(span);
        $('<span>').addClass(item.icon).appendTo(icon);
        $('<span>').html(item.name).appendTo(span);
        return span;
    }
}

$.extend(Page, {
    SysSiteMap: { component: new SysSiteMap(true) }
});