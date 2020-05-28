layui.config({ base: '/static/js/' }).define(function (exports) {
    var index = {
        base: layui.cache.base,
        extend: [
            'echarts',
            'echartsTheme',
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
    exports('index', index);
});

$(function () {
    $('[url]').each(function () {
        var $this = $(this), url = $this.attr('url');
        $this.html('');
        if (url) {
            $.get(url, function (res) {
                if ($this[0].tagName === 'SELECT') {
                    if ($this.attr('showAll')) {
                        $this.append('<option>全部</option>');
                    }
                    for (var i = 0; i < res.length; i++) {
                        $this.append('<option value="' + res[i].id + '">' + res[i].text + '</option>');
                    }
                }
            });
        }
    });
});