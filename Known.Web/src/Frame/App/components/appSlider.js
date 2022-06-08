function Slider(option) {
    //fields
    var _option = option,
        _elem,
        _handle,
        _interval = option.interval || 3000,
        _index = 0;

    //properties

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.destroy = function () {
        clearInterval(_handle);
    }

    //private
    function _init() {
        _elem = $('<div>').addClass('slider');

        if (_option.height)
            _elem.css({ height: _option.height });

        for (var i = 0; i < _option.items.length; i++) {
            var item = _option.items[i];
            $('<div>')
                .addClass('slider-item')
                .append('<img src="' + item.img + '">')
                .appendTo(_elem);
        }

        var snk = $('<div>').addClass('slider-snk').appendTo(_elem);
        for (var i = 0; i < _option.items.length; i++) {
            var item = _option.items[i];
            $('<span>').appendTo(snk);
        }

        _active(_index);
        _handle = setInterval(function () {
            _index++;
            if (_index === _option.items.length) {
                _index = 0;
            }
            _active(_index);
        }, _interval);
    }

    function _active(index) {
        _elem.find('.slider-item').removeClass('active fadeIn animated');
        _elem.find('.slider-item:eq(' + index + ')').addClass('active fadeIn animated');
        _elem.find('.slider-snk span').removeClass('active');
        _elem.find('.slider-snk span:eq(' + index + ')').addClass('active');
    }
}