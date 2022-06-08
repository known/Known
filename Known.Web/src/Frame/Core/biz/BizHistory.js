var BizHistory = {

    url: {
        GetUserHistory: baseUrl + '/Home/GetUserHistory'
    },
    data: {},

    init: function (option) {
        var _this = this;
        $.get(_this.url.GetUserHistory, function (res) {
            if (res) {
                if ($.isPlainObject(res)) {
                    _this.data = res;
                } else {
                    _this.data = JSON.parse(res);
                }
            }
        });
    }

}