///////////////////////////////////////////////////////////////////////
var Toolbar = function (tbId, handler) {
    this.tbId = tbId;

    var buttons = $('#' + tbId).find('.button,.mini-button,.mini-menuitem')
                               .unbind('click');
    for (var i = 0; i < buttons.length; i++) {
        var button = $(buttons[i]),
            buttonId = button[0].id;

        if (buttonId === '')
            continue;

        this[buttonId] = button.bind('click', function () {
            handler[$(this)[0].id].call(handler);
        });
    }
};