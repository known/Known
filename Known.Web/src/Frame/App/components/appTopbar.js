function Topbar(parent, option) {
    //fields
    var _option = option,
        _elem;

    //init
    _init();

    //properties
    this.elem = _elem;

    //methods
    this.setTitle = function (title, back) {
        var elb = _elem.find('.fa-chevron-left');
        if (!elb.length) {
            elb = $('<i>')
                .addClass('fa fa-chevron-left')
                .appendTo(_elem)
                .on('click', function () { app.router.back(); });
        }
        if (back) {
            elb.show();
        } else {
            elb.hide();
        }

        var el = _elem.find('.title');
        if (!el.length) {
            el = $('<span>').addClass('title').appendTo(_elem);
        }
        el.html(title);
    }

    this.showTop = function (hide) {
        if (hide) {
            app.router.elem.css({ top: 0 });
            _elem.css({ backgroundColor: 'transparent' });
        } else {
            app.router.elem.css({ top: '50px' });
            _elem.css({ backgroundColor: '#428bca' });
        }
    }

    this.setTool = function (tool) {
        var el = _elem.find('.topbar-tool');
        if (!el.length) {
            el = $('<div>').addClass('topbar-tool').appendTo(_elem);
        }
        el.html('');
        if (tool.length) {
            tool.appendTo(el);
        }
    }

    //private
    function _init() {
        _elem = $('<div>').addClass('topbar').appendTo(parent);
    }
}