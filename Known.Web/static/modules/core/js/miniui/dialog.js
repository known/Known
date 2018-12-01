///////////////////////////////////////////////////////////////////////
var Dialog = {

    show: function (option) {
        var dialog = mini.get('dialog');
        dialog.setTitle(option.title);
        dialog.setWidth(option.width || 500);
        dialog.setHeight(option.height || 300);
        dialog.show();

        var url = option.url || '/frame/partial';
        $('#dialog .mini-panel-body').loadHtml(url, {
            name: option.name, model: option.model || null
        }, function () {
            mini.parse();
            option.callback && option.callback();
        });
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
    },

    close: function (top = false) {
        if (top) {
            window.CloseOwnerWindow();
        } else {
            mini.get('dialog').hide();
        }
    }

};