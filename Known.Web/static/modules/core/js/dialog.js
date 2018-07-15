///////////////////////////////////////////////////////////////////////
var Dialog = {
    show: function (option) {
        var dialog = UI.get('dialog' + option.name);
        if (dialog) {
            option.callback && option.callback();
        } else {
            Ajax.getText(option.url, {
                name: option.name, model: option.model || null
            }, function (html) {
                $('body').append(html);
                mini.parse();
                option.callback && option.callback();
            });
        }
    },
    open: function (option) {
        var win = mini.open({
            url: option.url,
            showMaxButton: true,
            allowResize: true,
            title: option.title,
            width: option.width,
            height: option.height,
            onload: function () {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, 'load');
                }
            },
            ondestroy: function (action) {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, action);
                }
            }
        });
        option.max && win.max();
    }
};