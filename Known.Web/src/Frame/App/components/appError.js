function Error(option) {
    var _option = option || {};

    //methods
    this.render = function () {
        $('.topbar').hide();
        $('#app .router').css({ top: 0 });
        var elem = $('<div>').addClass('error-box');
        _init(elem);
        return elem;
    }

    //private
    function _init(elem) {
        var title = _option.title;
        var content = _option.content;

        if (!_option.title) {
            title = Language.Error500Title;
            content = Language.Error500Content;

            if (_option.type === '403') {
                title = Language.Error403Title;
                content = Language.Error403Content;
            } else if (_option.type === '404') {
                title = Language.Error404Title;
                content = Language.Error404Content;
            }
        }

        $('<h1>').html(_option.type).appendTo(elem);
        $('<h3>').html(title).appendTo(elem);
        $('<div>').html(content).appendTo(elem);
    }
}