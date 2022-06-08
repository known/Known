function Tabbar(parent, option) {
    //fields
    var _option = option,
        _elem;

    //init
    _init();

    //properties
    this.elem = _elem;

    //methods
    this.setData = function (data) {
        _setData(data);
    }

    this.showTab = function (show) {
        if (show) {
            app.router.elem.css({ bottom: '50px' });
            _elem.show();
        } else {
            app.router.elem.css({ bottom: 0 });
            _elem.hide();
        }
    }

    //private
    function _init() {
        _elem = $('<div>').addClass('tabbar').appendTo(parent);
        if (_option.data) {
            _setData(_option.data);
        }
    }

    function _setData(data) {
        var width = 100 / data.length + '%';
        for (var i = 0; i < data.length; i++) {
            var d = data[i];
            d.isTabbar = true;
            var link = $('<a>')
                .css({ width: width })
                .attr('id', d.id)
                .data('item', d)
                .appendTo(_elem)
                .on('click', function () {
                    var item = $(this).data('item');
                    _itemClick($(this), item);
                });
            $('<div>').addClass('icon')
                .append('<i class="' + d.icon + '">')
                .append('<span class="badge danger">')
                .appendTo(link);
            $('<div>').addClass('name').html(d.name).appendTo(link);
        }
        _itemClick(_elem.find('a:eq(0)'), data[0]);
    }

    function _itemClick(elem, item) {
        _elem.find('a').removeClass('active');
        elem.addClass('active');
        app.route(item);
    }
}