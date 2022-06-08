function Pager(option) {
    //fields
    var _option = option || {}, _this = this,
        _size = option.size || 10, _count = 1,
        _elem, _prev, _info, _next;

    //properties
    this.page = 1;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.search = function () {
        _clickPage(1);
    }

    this.setData = function (total) {
        _setData(total);
    }

    //private
    function _init() {
        _elem = $('<ul>').addClass('pager');
        _prev = $('<li>')
            .addClass('pager-prev')
            .html(Language.Previous)
            .appendTo(_elem)
            .on('click', function () {
                if (_this.page > 1) {
                    _clickPage(_this.page - 1);
                }
            });
        _info = $('<li>')
            .addClass('pager-info')
            .html('0/0')
            .appendTo(_elem);
        _next = $('<li>')
            .addClass('pager-next')
            .html(Language.Next)
            .appendTo(_elem)
            .on('click', function () {
                if (_this.page < _count) {
                    _clickPage(_this.page + 1);
                }
            });
    }

    function _setData(total) {
        _count = Math.ceil(total / _size);
        _info.html(_this.page + '/' + _count);
    }

    function _clickPage(page) {
        _this.page = page;
        _option.onPage(page);
    }
}