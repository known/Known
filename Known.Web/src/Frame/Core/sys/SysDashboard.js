function SysDashboard(option) {
    //fields
    var shortcut, commlink;

    //methods
    this.render = function (dom) {
        _createLeft(dom);
        _createRight(dom);
    }

    this.mounted = function () {
        if (option.countUrl) {
            $.get(option.countUrl, function (res) {
                for (var p in res) {
                    $('#' + p + ' h3').html(res[p]);
                }
            });
        }

        shortcut.load();
        commlink.load();
    }

    //private
    function _createLeft(elem) {
        var left = _createCol(elem, '75%');

        //left row1
        var row1 = $('<div>').addClass('row').appendTo(left);
        var iconRow = $('<div>').addClass('card').appendTo(row1);
        var icons = option.icons || [{}];
        Dashboard.createIconCards(iconRow, icons);

        //left row2
        var row2 = $('<div>').addClass('row').appendTo(left);
        var card = $('<div>').addClass('card').css({ height: '376px' }).appendTo(row2);
        var menus = option.menus || [];
        for (var i = 0; i < menus.length; i++) {
            var menu = menus[i];
            _createMenu(card, menu);
        }
    }

    function _createMenu(card, menu) {
        var map = $('<div>').addClass('dash-sitemap').appendTo(card);
        _createMenuItem('<span>', map, menu)
            .addClass('dash-menu').css({ top: 0, left: 0 });
        var div = $('<ul>').addClass('menu-items').css({ paddingTop: '5px' }).appendTo(map);
        for (var i = 0; i < menu.children.length; i++) {
            var item = menu.children[i];
            _createMenuItem('<li>', div, item).click(function () {
                var d = $(this).data('item');
                d.type = 'page';
                d.id = d.code;
                d.title = d.name;
                showTabPage(d);
            });
        }
    }

    function _createMenuItem(tagName, div, item) {
        var span = $(tagName).data('item', item).appendTo(div);
        var icon = $('<span>').addClass('icon').appendTo(span);
        $('<span>').addClass(item.icon).appendTo(icon);
        $('<span>').html(item.name).appendTo(span);
        return span;
    }

    function _createRight(elem) {
        var right = _createCol(elem, '25%');

        //right row1
        var row1 = _createRow(right);
        shortcut = Dashboard.createShortcut(row1, { height: '211px' });

        //right row2
        var row2 = _createRow(right);
        commlink = Dashboard.createCommLink(row2, { height: '212px' });
    }

    function _createRow(right) {
        return $('<div>').addClass('row').css('padding-left', '0').appendTo(right);
    }

    function _createCol(dom, width) {
        return $('<div>').css({ float: 'left', width: width }).appendTo(dom);
    }
}