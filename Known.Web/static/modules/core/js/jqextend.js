$.fn.extend({

    loadHtml: function (url, param, callback) {
        var _this = $(this).html('加载中....');
        Ajax.getText(url, param, function (result) {
            if (!$.isPlainObject(result)) {
                _this.html(result);
                mini.parse();
                callback && callback();
            }
        });
    }

});