///////////////////////////////////////////////////////////////////////
var Ajax = {

    _request: function (type, dataType, args) {
        var url = args[0],
            data = null,
            callback = null;

        if (args.length > 2) {
            data = args[1];
            callback = args[2];
        } else if (args.length > 1) {
            if (typeof args[1] === 'function') {
                callback = args[1];
            } else {
                data = args[1];
            }
        }

        $.ajax({
            type: type, dataType: dataType,
            url: url, data: data,
            cache: false, async: true,
            success: function (result) {
                callback && callback(result);
            }
        });
    },

    getText: function () {
        this._request('get', 'text', arguments);
    },

    postText: function () {
        this._request('post', 'text', arguments);
    },

    getJson: function () {
        this._request('get', 'json', arguments);
    },

    postJson: function () {
        this._request('post', 'json', arguments);
    }

};