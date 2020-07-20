layui.config({ base: '/static/js/' }).define(function (exports) {
    var index = {
        base: layui.cache.base,
        extend: [
            'echarts',
            'echartsTheme'
        ],
        extend1: [
            'common',
            'admin',
            'frame'
        ]
    };
    layui.each(index.extend, function (a, i) {
        var n = {};
        n[i] = '{/}' + index.base + 'extend/' + i;
        layui.extend(n);
    });
    layui.cache.base = '/';
    layui.each(index.extend1, function (a, i) {
        var n = {};
        n[i] = 'Content/js/' + i;
        layui.extend(n);
    });
    exports('index', index);
});