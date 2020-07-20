layui.define('common', function (exports) {
    var $ = layui.jquery,
        layer = layui.layer,
        form = layui.form,
        table = layui.table,
        common = layui.common;

    function getCurTab() {
        return top.layui.admin.getCurTab();
    }

    function Grid(option) {
        var name = option.name,
            config = {},
            toolbar = option.toolbar;
        var tableIns = null, _where = {}, keyId = name + '_key';
        var _this = this;

        $.extend(config, {
            elem: '#' + name, skin: 'line',
            even: true, page: true, cellMinWidth: 80,
            defaultToolbar: ['filter', 'print']
        }, option.config);
        config.height = config.height || 'full-25';

        if (toolbar) {
            if (!config.toolbar) {
                var tab = getCurTab();
                var tbHtml = '<div>';
                if (tab.module && tab.module.children) {
                    tab.module.children.forEach(function (d) {
                        tbHtml += ('<button class="layui-btn layui-btn-sm" lay-event="' + d.code + '">');
                        tbHtml += ('<i class="layui-icon ' + d.icon + '"></i><span>' + d.name + '</span>');
                        tbHtml += '</button>';
                    });
                }
                if (config.url || option.showSearch) {
                    //tbHtml += '<span class="grid-search-adv">高级</span>';
                    tbHtml += '<span class="grid-search">';
                    tbHtml += '  <input type="text" id="' + keyId + '" placeholder="请输入查询关键字" class="layui-input" autocomplete="off">';
                    tbHtml += '  <i class="layui-icon layui-icon-search" lay-event="search"></i>';
                    tbHtml += '</span>';

                    toolbar.search = function () {
                        var key = $('#' + keyId).val();
                        _this.reload({ key: key });
                        $('#' + keyId).val(key);
                    }
                }
                tbHtml += '</div>';
                config.toolbar = tbHtml;
            }

            table.on('toolbar(' + name + ')', function (obj) {
                var type = obj.event;
                var rows = table.checkStatus(name).data;
                toolbar[type] && toolbar[type].call(this, new GridManager(_this, rows));
            });

            table.on('tool(' + name + ')', function (obj) {
                var type = obj.event;
                toolbar[type] && toolbar[type].call(this, new GridManager(_this, [obj.data]));
            });
        }

        if (config.url) {
            config.method = 'post';
            config.autoSort = false;
            tableIns = table.render(config);

            table.on('sort(' + name + ')', function (obj) {
                var key = $('#' + keyId).val();
                _where.field = obj.field;
                _where.order = obj.type;
                tableIns.reload({ initSort: obj, where: _where });
                $('#' + keyId).val(key);
            });
        }

        this.setData = function (data) {
            config.data = data;
            tableIns = table.render(config);
        }

        this.where = {};
        this.reload = function (where) {
            $.extend(this.where, where);
            _where.query = JSON.stringify(this.where);
            tableIns.reload({ where: _where });
        }
    }

    function GridManager(grid, rows) {
        this.grid = grid;
        this.rows = rows;
        this.row = rows[0];

        this.selectRow = function (callback) {
            if (!rows || rows.length === 0 || rows.length > 1) {
                layer.msg('请选择一条记录！');
                return;
            }

            var row = rows[0];
            callback && callback({ grid: grid, rows: rows, row: row, ids: [row.Id] });
        }

        this.selectRows = function (callback) {
            if (!rows || rows.length === 0) {
                layer.msg('请至少选择一条记录！');
                return;
            }

            var ids = rows.map(function (d) { return d.Id; });
            callback && callback({ grid: grid, rows: rows, ids: ids });
        }

        this.editRow = function (form, ext) {
            this.selectRow(function (e) {
                form.show(e.row, ext);
            });
        }

        function deleteData(e, url, callback) {
            var length = e.rows.length;
            var msg = length > 1 ? ('所选的' + length + '条记录') : '该记录';
            common.confirm('确定要删除' + msg + '吗？', function () {
                common.post(url, { data: JSON.stringify(e.ids) }, function () {
                    e.grid.reload();
                    callback && callback(e);
                });
            });
        }

        this.deleteRow = function (url, callback) {
            this.selectRow(function (e) {
                deleteData(e, url, callback);
            });
        }

        this.deleteRows = function (url, callback) {
            this.selectRows(function (e) {
                deleteData(e, url, callback);
            });
        }
    }

    function Field(form, elem) {
        this.form = form;
        this.elem = elem;
        this.id = elem[0].id;
        this.name = elem[0].name;
        this.type = elem[0].type;

        function addError(elm) {
            if (elm[0].type === 'radio')
                elm.parent().addClass('error');
            else
                elm.addClass('error');
        }

        this.setReadonly = function (readonly) {
            if (readonly) {
                this.elem.attr('readonly', 'readonly');
            } else {
                this.elem.removeAttr('readonly');
            }
        }

        this.clearError = function () {
            if (this.type === 'radio')
                this.elem.parent().removeClass('error');
            else
                this.elem.removeClass('error');
        }

        this.validate = function () {
            this.clearError();
            var value = $.trim(this.getValue());
            if (!elem[0].disabled && elem[0].required && value === '') {
                addError(this.elem);
                return false;
            }
            return true;
        }

        this.getValue = function () {
            if (this.type === 'checkbox') {
                return this.elem[0].checked ? 1 : 0;
            } else if (this.type === 'radio') {
                return $('input[name="' + this.name + '"]:checked').val();
            } else {
                return this.elem.val();
            }
        }

        this.setValue = function (value) {
            if (this.type === 'checkbox') {
                this.elem[0].checked = value === 1;
            } else if (this.type === 'radio') {
                if (!value || value === '')
                    $('input[name="' + this.name + '"]').attr('checked', false);
                else
                    $('input[name="' + this.name + '"][value="' + value + '"]').attr('checked', true);
            } else {
                if (isNaN(value) && !isNaN(Date.parse(value))) {
                    var format = this.elem[0].placeholder;
                    if (format) {
                        value = new Date(value).format(format);
                    }
                }
                this.elem.val(value);
            }
        }
    }

    function Form(option) {
        var name = option.name,
            config = option.config || {},
            toolbar = option.toolbar;

        var _this = this;
        var btn = [], handler = {}, btnHtml = '';
        if (toolbar) {
            for (var i = 0; i < toolbar.length; i++) {
                var text = toolbar[i].text;
                btn.push(text);
                var evt = i === 0 ? 'yes' : ('btn' + (i + 1));
                btnHtml += '<a class="layui-btn layui-btn-normal" data-type="' + evt + '">' + text + '</a>';
                var hdl = toolbar[i].handler;
                handler[evt] = function () {
                    hdl && hdl.call(this, _this);
                };
            }
        }

        handler['btn' + (btn.length + 1)] = function (index) {
            if (config.area) {
                layer.close(index);
            } else {
                $('#' + name).hide();
            }
        };
        btn.push('关闭');

        if (config.area) {
            $.extend(config, { btn: btn }, handler);
        } else {
            var footer = $('#' + name + ' .form-card-footer');
            if (footer.length) {
                btnHtml += '<a class="layui-btn layui-btn-primary" data-type="btn' + btn.length + '">关闭</a>';
                $('#' + name + ' .form-card-footer').html(btnHtml);
                $('#' + name + ' .form-card-footer .layui-btn').on('click', function () {
                    var othis = $(this), type = othis.data('type');
                    handler[type] ? handler[type].call(this, othis) : '';
                });
            }
        }

        if (option.url) {
            $.get(option.url, function (data) {
                _this.setData(data, config.init);
            });
        }

        var index = 0, fields = [], open = false;
        this.show = function (data, ext) {
            open = true;
            data = data || (option.defData || { Id: '' });
            var title = option.title;
            if (!title) {
                var tab = getCurTab();
                title = tab ? tab.title : '';
            }
            ext = ext || {};
            var info = ext.title || '';
            var actionName = data.Id === '' ? '【新增】' : '【编辑】';
            if (info.indexOf('【') > -1) {
                actionName = '';
            }
            title = title + actionName + info;
            if (config.area) {
                config.title = title;
                config.success = function () {
                    _this.setData(data, config.init);
                }
                index = common.open(config);
            } else {
                $('#' + name + ' .form-card-header').html(title);
                this.setData(data, config.init);
                $('#' + name).show();
            }
        }

        this.initFields = function () {
            fields.length = 0;
            var inputs = $('#' + name).find('input,select,textarea');
            $.each(inputs, function (_, item) {
                var elem = $(item);
                if (elem.length) {
                    var name = elem.attr('name');
                    var exist = fields.filter(function (f) { return f.name === name; });
                    if (exist.length === 0) {
                        var field = new Field(form, elem);
                        _this[name] = field;
                        fields.push(field);
                    }
                }
            });
        }

        this.close = function () {
            if (!open) return;
            if (index > 0) {
                layer.close(index);
            } else {
                $('#' + name).hide();
            }
        }

        this.validate = function () {
            var valid = true;
            $.each(fields, function (_, item) {
                if (!item.validate()) {
                    valid = false;
                }
            });
            return valid;
        }

        this.clear = function () {
            $.each(fields, function (_, item) {
                item.clearError();
                item.setValue('');
            });
        }

        this.getData = function () {
            var data = {};
            $.each(fields, function (_, item) {
                data[item.name] = item.getValue();
            });
            return data;
        }

        this.setData = function (data, callback) {
            this.clear();
            for (var p in data) {
                var field = fields.filter(function (f) { return f.name === p; });
                if (field.length) {
                    field[0].setValue(data[p]);
                }
            }
            var e = { form: _this, data: data };
            config.setData && config.setData(e);
            callback && callback(e);
        }

        this.save = function (url, callback) {
            if (!_this.validate())
                return;

            var data = _this.getData();
            common.post(url, { data: JSON.stringify(data) }, function (id) {
                data.Id = id;
                _this.setData(data);
                _this.close();
                callback && callback();
            });
        }

        this.initFields();
    }

    function selectIcon(icon, callback) {
        var data = [
            'heart-fill', 'heart', 'light', 'time', 'bluetooth', 'at', 'mute',
            'mike', 'key', 'gift', 'email', 'rss', 'wifi', 'logout',
            'android', 'ios', 'windows', 'transfer', 'service', 'subtraction', 'addition',
            'slider', 'print', 'export', 'cols', 'screen-restore', 'screen-full', 'rate-half',
            'rate', 'rate-solid', 'cellphone', 'vercode', 'login-wechat', 'login-qq', 'login-weibo',
            'password', 'username', 'refresh-3', 'auz', 'spread-left', 'shrink-right', 'snowflake',
            'tips', 'note', 'home', 'senior', 'refresh', 'refresh-1', 'flag',
            'theme', 'notice', 'website', 'console', 'face-surprised', 'set', 'template-1',
            'app', 'template', 'praise', 'tread', 'male', 'female', 'camera',
            'camera-fill', 'more', 'more-vertical', 'rmb', 'dollar', 'diamond', 'fire',
            'return', 'location', 'read', 'survey', 'face-smile', 'face-cry', 'cart-simple',
            'cart', 'next', 'prev', 'upload-drag', 'upload', 'download-circle', 'component',
            'file-b', 'user', 'find-fill', 'loading', 'loading-1', 'add-1', 'play',
            'pause', 'headset', 'video', 'voice', 'speaker', 'fonts-del', 'fonts-code',
            'fonts-html', 'fonts-strong', 'unlink', 'picture', 'link', 'face-smile-b', 'align-left',
            'align-right', 'align-center', 'fonts-u', 'fonts-i', 'tabs', 'radio', 'circle',
            'edit', 'share', 'delete', 'form', 'cellphone-fine', 'dialogue', 'fonts-clear',
            'layer', 'date', 'water', 'code-circle', 'carousel', 'prev-circle', 'layouts',
            'util', 'templeate-1', 'upload-circle', 'tree', 'table', 'chart', 'chart-screen',
            'engine', 'triangle-d', 'triangle-r', 'file', 'set-sm', 'reduce-circle', 'add-circle',
            '404', 'about', 'up', 'down', 'left', 'right', 'circle-dot',
            'search', 'set-fill', 'group', 'friends', 'reply-fill', 'menu-fill', 'log',
            'picture-fine', 'face-smile-fine', 'list', 'release', 'ok', 'help', 'chat',
            'top', 'star', 'star-fill', 'close-fill', 'close', 'ok-circle', 'add-circle-fine'
        ];
        var content = '<ul class="icon-list">';
        data.forEach(function (d) {
            var icon1 = 'layui-icon-' + d;
            content += '<li id="' + icon1 + '"';
            content += icon1 === icon ? ' class="active">' : '>';
            content += '<i class="layui-icon ' + icon1 + '"></i>';
            content += '</li>';
        });
        content += '</ul>';
        common.open({
            title: '选择图标',
            area: ['400px', '250px'],
            content: content,
            btn: ['确定', '关闭'],
            yes: function (index) {
                var icon = $('.icon-list li.active').attr('id');
                layer.close(index);
                callback && callback(icon);
            },
            btn2: function (index) {
                layer.close(index);
            },
            success: function (layero, index) {
                $('.icon-list li').click(function () {
                    $('.icon-list li').removeClass('active');
                    $(this).addClass('active');
                });
            }
        });
    }

    exports('frame', {

        common: common,

        selectIcon: function (icon, callback) {
            selectIcon(icon, callback);
        },

        grid: function (option) {
            return new Grid(option);
        },

        form: function (option) {
            return new Form(option);
        },

        open: function (option) {
            return common.open(option);
        },

        confirm: function (message, callback) {
            common.confirm(message, callback);
        },

        post: function (url, data, callback) {
            common.post(url, data, callback);
        }

    });
});