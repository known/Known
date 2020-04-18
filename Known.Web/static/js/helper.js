layui.define(function (exports) {
    var $ = layui.jquery,
        layer = layui.layer,
        form = layui.form,
        table = layui.table;

    function Grid(option) {
        var name = option.name,
            config = option.config,
            toolbar = option.toolbar;

        $.extend(config, { skin: 'line', page: true, height: 'full-25' });

        config.elem = '#' + name;
        if (!config.initSort) {
            config.initSort = { field: 'CreateTime', type: 'desc' };
        }
        if (config.url) {
            config.method = 'post';
            table.render(config);
        }

        if (toolbar) {
            var _this = this;
            table.on('toolbar(' + name + ')', function (obj) {
                var type = obj.event;
                var rows = table.checkStatus(name).data;
                toolbar[type] && toolbar[type].call(this, { grid: _this, rows: rows });
            });
            table.on('tool(' + name + ')', function (obj) {
                var type = obj.event;
                toolbar[type] && toolbar[type].call(this, { grid: _this, row: obj.data });
            });
        }

        this.setData = function (data) {
            config.data = data;
            table.render(config);
        }

        this.deleteRows = function (rows, callback) {
            if (!rows || rows.length === 0) {
                layer.msg('请至少选择一条记录！');
                return;
            }

            var ids = [];
            rows.forEach(function (d) {
                ids.push(d.Id);
            });

            var msg = rows.length > 1 ? ('所选的' + rows.length + '条记录') : '该记录';
            layer.confirm('确定要删除' + msg + '吗？', function (index) {
                layer.close(index);
                var data = JSON.stringify(ids);
                callback && callback(data);
            });
        }
    }

    function Form(option) {
        var name = option.name,
            title = option.title,
            config = option.config,
            toolbar = option.toolbar;

        var _this = this;
        var btn = [], handler = {};
        if (toolbar) {
            for (var i = 0; i < toolbar.length; i++) {
                btn.push(toolbar[i].text);
                var evt = i === 0 ? 'yes' : ('btn' + (i + 1));
                var hdl = toolbar[i].handler;
                handler[evt] = function () {
                    hdl && hdl.call(this, { form: _this });
                };
            }
        }

        handler['btn' + (btn.length + 1)] = function () {
            layer.close(layer.index);
        };
        btn.push('关闭');

        $.extend(config, { type: 1, shade: 0, btn: btn }, handler);

        this.show = function (data) {
            config.title = title + (data.Id === '' ? '【新增】' : '【编辑】');
            config.success = function (layero, index) {
                form.render(null, name);
                _this.setData(data, config.init);
            }
            layer.open(config);
        }

        this.getData = function () {
            return form.val(name);
        }

        this.setData = function (data, callback) {
            form.val(name, data);
            var e = { form: _this, data: data };
            config.setData && config.setData(e);
            callback && callback(e);
        }
    }

    var helper = {
        fullScreen: function () {
            var el = document.documentElement;
            var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
            if (rfs) {
                rfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE，这里其实就是模拟了按下键盘的F11，使浏览器全屏
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
        },
        exitScreen: function () {
            var el = document;
            var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
            if (cfs) {
                cfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE，这里和fullScreen相同，模拟按下F11键退出全屏
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
        },
        toTree: function (arr, rootId) {
            arr.forEach(function (element) {
                var parentId = element.pid;
                if (parentId) {
                    arr.forEach(function (ele) {
                        if (ele.id === parentId) {
                            if (!ele.children) {
                                ele.children = [];
                            }
                            ele.children.push(element);
                        }
                    });
                }
            });
            arr = arr.filter(function (ele) {
                return ele.pid === rootId;
            });
            return arr;
        },
        grid: function (option) {
            return new Grid(option);
        },
        form: function (option) {
            return new Form(option);
        }
    };

    exports('helper', helper);
});