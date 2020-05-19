layui.define('layer', function (exports) {
    var $ = layui.jquery,
        layer = layui.layer;

    var Common = {

        list2Tree: function (data, pid) {
            return data.filter(father => {
                let branchArr = data.filter(child => father['id'] === child['pid']);
                branchArr.length > 0 ? father['children'] = branchArr : '';
                return father['pid'] === pid;
            });
        },

        open: function (option) {
            var type = 1;
            if (option.url) {
                type = 2;
                option.content = option.url;
            }
            $.extend(option, { type: type });
            return layer.open(option);
        },

        confirm: function (message, callback) {
            layer.confirm(message, function (index) {
                callback && callback();
                layer.close(index);
            });
        },

        post: function (url, data, callback) {
            $.post(url, data, function (result) {
                layer.msg(result.message);
                if (result.ok) {
                    callback && callback(result.data);
                }
            });
        },

        download: function (url, data, callback) {
            var tokenKey = 'downloadToken';
            var tokenValue = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
                var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
                return v.toString(16);
            });
            function getCookie(name) {
                var reg = new RegExp('(^| )' + name + '=([^;]*)(;|$)');
                var arr = document.cookie.match(reg);
                if (arr && arr.length > 3)
                    return unescape(arr[2]);
                return null;
            }
            function loading() {
                var index = layer.load(2);
                var downloadTimer = window.setInterval(function () {
                    var token = getCookie(tokenKey);
                    if (token === tokenValue) {
                        window.clearInterval(downloadTimer);
                        layer.close(index);
                        callback && callback();
                    }
                }, 1000);
            }

            var form = $('<form method="post" action="' + url + '" target="" style="display:none">').appendTo('body');
            $('<input type="hidden">').attr('name', tokenKey).attr('value', tokenValue).appendTo(form);
            if (data && data !== null) {
                for (var p in data) {
                    $('<input type="hidden">').attr('name', p).attr('value', data[p]).appendTo(form);
                }
            }
            form.submit();
            loading();
        },

    };

    exports('common', Common);
});