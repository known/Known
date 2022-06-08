function Query(option) {
    //fields
    var _option = option,
        _elem = $('<div>').addClass('query'),
        _inputs = [],
        _this = this;

    //properties
    this.elem = _elem;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.getData = function () {
        var data = {};
        for (var i = 0; i < _inputs.length; i++) {
            var input = _inputs[i];
            data[input.id] = input.getValue();
        }
        return data;
    }

    this.search = function () {
        _search();
    }

    //pricate
    function _init() {
        if (_option.querys && _option.querys.length) {
            _setQuery(_option.querys);
        }
    }

    function _setQuery(querys) {
        _elem.html('');
        for (var i = 0; i < querys.length; i++) {
            var q = querys[i];
            var item = $('<div>').addClass('query-item').appendTo(_elem);
            var label = $('<label>').html(q.title + '：').appendTo(item);

            if (q.labelWidth) {
                label.css({ width: q.labelWidth });
            }

            if (q.type === 'dateRange') {
                var now = new Date();
                var sDate = new Date(now.getFullYear(), now.getMonth(), now.getDate() - (q.diffDay || 7));
                var start = new Input(item, $.extend({}, q, {
                    type: 'date',
                    id: 'start' + q.field,
                    value: sDate.format(DateFormat)
                }));
                _setInput(start);
                var end = new Input(item, $.extend({}, q, {
                    type: 'date',
                    id: 'end' + q.field,
                    value: now.format(DateFormat)
                }));
                _setInput(end);
            } else {
                var input = new Input(item, q);
                _setInput(input);
            }
        }

        Utils.createButton({
            text: Language.Query, handler: function () {
                _search();
            }
        }).appendTo(_elem.find('.query-item').last());
    }

    function _setInput(input) {
        _inputs.push(input);
        _this[input.id] = input;
    }

    function _search() {
        var data = _this.getData();
        if (_option.onSearch) {
            _option.onSearch({ grid: _option.grid, data: data });
        } else {
            _option.grid.reload(data);
        }
    }
}