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

        tree2List: function (data, pid) {
            return data.reduce((arr, { id, title, children = [] }) =>
                arr.concat([{ id, title, pid }], this.tree2List(children, id)), []);
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
        }

    };

    exports('common', Common);
});