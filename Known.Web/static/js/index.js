layui.extend({
    setter: 'config',
    admin: 'admin'
}).define(['setter', 'admin'], function (exports) {
    var setter = layui.setter;
    layui.config({
        base: setter.base + 'modules/'
    });
    layui.each(setter.extend, function (a, i) {
        var n = {};
        n[i] = '{/}' + setter.base + 'extend/' + i;
        layui.extend(n);
    });

    exports('index', {});
});