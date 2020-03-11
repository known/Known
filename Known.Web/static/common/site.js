//---------------------------jQuery-------------------------------------------//
var cachedPages = [];

$.fn.extend({

    loadHtml: function () {
        var url = arguments[0],
            param = null,
            callback = null;

        if (arguments.length > 2) {
            param = arguments[1];
            callback = arguments[2];
        } else if (arguments.length > 1) {
            if (typeof arguments[1] === 'function') {
                callback = arguments[1];
            } else {
                param = arguments[1];
            }
        }

        var _this = $(this), pageUrl = url + JSON.stringify(param);
        var page = cachedPages.find(p => p.url === pageUrl);

        if (!page) {
            _this.html('加载中....');
            if (!url.startWith('/'))
                url = '/' + url;
            if (url.indexOf('.html') === -1)
                url = url + '.html';

            Ajax.getText('/pages' + url, param, function (result) {
                if (!$.isPlainObject(result)) {
                    cachedPages.push({ url: pageUrl, html: result });
                    _this.html(result);
                    callback && callback();
                }
            });
        } else {
            _this.html(page.html);
            callback && callback();
        }
    },

    loadView: function (url, callback) {
        if (!url)
            return;

        //var arr = url.split('/');
        //var view = arr[arr.length - 1].split('.')[0];
        $(this).loadHtml(url, function () {
            callback && callback();
        });
    }

});

//---------------------------Ajax---------------------------------------------//
$(document).ajaxSend(function (event, xhr, settings) {
    var user = User.getUser();
    if (user) {
        xhr.setRequestHeader('Authorization', 'Token ' + user.Token);
    }
});
$(document).ajaxError(function (event, xhr, settings, exception) {
    if (xhr.status === 401) {
        top.location = '/login';
    }
});

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
    },

    action: function (name, url, data, callback) {
        Message.mask('数据' + name + '中...');
        //data = { '': JSON.stringify(data) };
        Ajax.postJson(url, data, function (result) {
            Message.unmask();
            Message.result(result, function (d) {
                callback && callback(d);
            });
        });
    },

    download: function (url, data, callback) {
        var tokenKey = 'downloadToken';
        var tokenValue = guid();
        function loading() {
            Message.mask('文件下载中，请稍后......');
            var downloadTimer = window.setInterval(function () {
                var token = $.cookie(tokenKey);
                if (token === tokenValue) {
                    window.clearInterval(downloadTimer);
                    Message.unmask();
                    callback && callback();
                }
            }, 1000);
        }
        var form = $('<form>').attr({ style: 'display:none', target: '', method: 'post', action: url }).appendTo('body');
        $('<input>').attr({ type: 'hidden', name: tokenKey, value: tokenValue }).appendTo(form);
        if (data) {
            for (var p in data) {
                $('<input>').attr({ type: 'hidden', name: p, value: data[p] }).appendTo(form);
            }
        }
        form.submit();
        loading();
    }

};

//---------------------------Code---------------------------------------------//
var Code = {

    key: 'known_codes',

    setData: function (codes) {
        localStorage.setItem(this.key, JSON.stringify(codes));
    },

    getCodes: function (type) {
        var value = localStorage.getItem(this.key);
        if (value) {
            return JSON.parse(value)[type];
        }
        return null;
    },

    getCode: function (type, id) {
        var codes = this.getCodes(type);
        if (codes) {
            return codes.find(c => c.id === id.toString());
        }
        return null;
    }

};

//---------------------------Url----------------------------------------------//
var Url = {

    getParam: function (name) {
        var url = document.location.href;
        var arrs = url.split('?');
        if (arrs.length <= 1)
            return '';

        var paras = arrs[1].split('&');
        var para;
        for (var i = 0; i < paras.length; i++) {
            para = paras[0].split('=');
            if (para !== null && para[0] === name)
                return para[1];
        }

        return '';
    }

};

//---------------------------Message------------------------------------------//
var Message = {

    show: function (option) {
        var options = $.extend({
            width: '300px', height: 'auto', showType: 'fade'
        }, option);
        $.messager.show(options);
    },

    tip: function (message, position = 'topCenter', showType = 'slide') {
        var style, bt = document.body.scrollTop, dt = document.documentElement.scrollTop;
        switch (position) {
            case 'topLeft':
                style = { top: bt + bt, bottom: '', left: 0, right: '' };
                break;
            case 'topCenter':
                style = { top: bt + dt, bottom: '', right: '' };
                break;
            case 'topRight':
                style = { top: bt + dt, bottom: '', left: '', right: 0 };
                break;
            case 'centerLeft':
                style = { bottom: '', left: 0, right: '' };
                break;
            case 'center':
                style = { bottom: '', right: '' };
                break;
            case 'centerRight':
                style = { bottom: '', left: '', right: 0 };
                break;
            case 'bottomLeft':
                style = { top: '', bottom: -bt - dt, left: 0, right: '' };
                break;
            case 'bottomCenter':
                style = { top: '', bottom: -bt - dt, right: '' };
                break;
            case 'bottomRight':
                break;
        }
        this.show({ msg: message, width: '250px', showType: showType, style: style });
    },

    alert: function (message, icon = 'info', callback) {
        $.messager.alert('提示', message, icon, callback);
    },

    warning: function (message, callback) {
        $.messager.alert('警告', message, 'warning', callback);
    },

    error: function (message, callback) {
        $.messager.alert('错误', message, 'error', callback);
    },

    confirm: function (message, callback) {
        $.messager.confirm('确认', message, function (r) {
            r && callback && callback(r);
        });
    },

    prompt: function (message, callback) {
        $.messager.prompt('提示', message, function (r) {
            r && callback && callback(r);
        });
    },

    mask: function (message) {
        $.messager.progress({ text: message });
    },

    unmask: function () {
        $.messager.progress('close');
    },

    result: function (rtn, callback) {
        if (!rtn.ok) {
            this.error(rtn.message, function () {
                callback && callback(rtn.data);
            });
        } else {
            this.tip(rtn.message);
            callback && callback(rtn.data);
        }
    }

};

//---------------------------Dialog-------------------------------------------//
var Dialog = {

    show: function (option) {
        var options = $.extend({
            width: '500px', height: 'auto', showType: 'fade', iconCls: 'fa-tv', cls: 'dialog',
            collapsible: false, minimizable: false, maximizable: true, resizable: true, modal: true
        }, option);

        var dlg = $('#' + option.id).dialog(options);
        dlg.dialog('open');
        return dlg.dialog('dialog');
    },

    close: function (id) {
        $('#' + id).dialog('close');
    },

    form: function (option) {
        var page = top.MainView.currentTab || {};
        var actionName = '【新增】';
        if (option.data.Oid !== '') {
            actionName = '【编辑】';
        } else if (option.data.IsCopy) {
            actionName = '【复制】';
        }
        var title = (option.moduleName || page.text) + actionName;
        $.extend(option, { title: title });
        this.show(option);
    }

};

//---------------------------Toolbar------------------------------------------//
var Toolbar = {

    buttons: [
        { id: 'add', text: '新增', iconCls: 'fa-plus' },
        { id: 'edit', text: '编辑', iconCls: 'fa-pencil' },
        { id: 'remove', text: '删除', iconCls: 'fa-minus' },
        { id: 'imports', text: '导入', iconCls: 'fa-sign-in' },
        { id: 'exports', text: '导出', iconCls: 'fa-sign-out' },
        { id: 'upload', text: '上载', iconCls: 'fa-upload' },
        { id: 'download', text: '下载', iconCls: 'fa-download' }
    ],

    find: function (id) {
        return this.buttons.find(b => b.id === id);
    },

    bindById: function (tbId, obj) {
        this.bind('#' + tbId, obj);
    },

    bind: function (selector, obj) {
        for (var p in obj) {
            bindButton(selector, p, obj);
        }

        function bindButton(selector, name, obj) {
            var btn = $(selector + ' #' + name);
            if (btn.length) {
                btn.unbind('click').bind('click', function () {
                    obj[name].call(obj);
                });
            }
        }
    }

};

//---------------------------Form---------------------------------------------//
var Form = function (formId, option) {
    this.formId = formId;
    this.option = option || {};

    var _form = new mini.Form('#' + formId);
    $.extend(true, this, _form);

    //public
    this.clear = function (controls) {
        if (controls) {
            $(controls.split(',')).each(function (i, c) {
                var control = mini.getbyName(c, _form);
                if (control) {
                    control.setValue('');
                    if (control.type === 'autocomplete') {
                        control.setText('');
                    }
                }
            });
        } else {
            _form.clear();
        }
    };

    this.validate = function (tabsId, tabIndex) {
        if (_form.validate())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(tabIndex);
            tabs.activeTab(tab);
        }
        return false;
    };

    this.getData = function (encode) {
        var data = _form.getData(true);
        return encode ? mini.encode(data) : data;
    };

    this.setData = function (data, callback) {
        if (data) {
            _form.setData(data);
            callback && callback(this, data);
            _form.setChanged(false);
            this.bindEnterJump();
        }
    };

    this.bindEnterJump = function () {
        var inputs = _form.getFields();
        var activeIndexes = getActiveIndexes(inputs);

        for (var i = 0, len = activeIndexes.length; i < len; i++) {
            (function (i) {
                var index = activeIndexes[i];
                var nextIndex = activeIndexes[i + 1];

                if (i === len - 1) {
                    nextIndex = activeIndexes[0];
                }

                var current = inputs[index];
                $(current.getEl()).keyup(function (e) {
                    if (e.keyCode === 13) {
                        selectInput(inputs[nextIndex]);
                    } else if (i > 0 && e.keyCode === 38) {
                        if (current.type !== 'textarea' && (
                            current.type !== 'autocomplete' &&
                            current.type !== 'combobox' ||
                            !current.isShowPopup()
                        )) {
                            selectInput(inputs[activeIndexes[i - 1]]);
                        }
                    }
                });
            })(i);
        }

        function selectInput(input) {
            setTimeout(function () {
                input.focus();
                if (input.type !== 'textarea') {
                    input.selectText();
                }
            }, 10);
        }

        function getActiveIndexes(inputs) {
            var indexes = [];
            for (var i = 0, len = inputs.length; i < len; i++) {
                var input = inputs[i];
                $(input.getEl()).unbind('keyup');

                if (input.type !== 'hidden' &&
                    input.type !== 'checkbox' &&
                    input.type !== 'checkboxlist' &&
                    input.type !== 'radiobuttonlist' &&
                    input.type !== 'htmlfile' &&
                    input.getEnabled() === true &&
                    input.getVisible() === true)
                    indexes.push(i);
            }
            return indexes;
        }
    };

    this.model = function (isLabel) {
        var labelClass = 'form-input-label-model';
        $('span.' + labelClass).remove();
        var inputs = this.getFields();
        for (var i = 0, len = inputs.length; i < len; i++) {
            var input = inputs[i];
            input.setVisible(!isLabel);

            if (input.type === 'hidden' || !isLabel)
                continue;

            var text = input.getValue();
            if (input.type === 'combobox' ||
                input.type === 'autocomplete' ||
                input.type === 'listbox' ||
                input.type === 'checkbox' ||
                input.type === 'checkboxlist' ||
                input.type === 'radiobuttonlist' ||
                input.type === 'datepicker' ||
                input.type === 'timespinner') {
                text = input.getText();
            } else if (input.type === 'textarea') {
                text = text.htmlEncode();
            }

            var html = '<span class="' + labelClass + '">' + text + '</span>';
            $(input.getEl()).after(html);
        }
    };

    //private
    function init() {
        var inputs = this.getFields();
        for (var i = 0; i < inputs.length; i++) {
            var input = inputs[i];
            if (input.type === 'combobox' ||
                input.type === 'checkboxlist' ||
                input.type === 'radiobuttonlist') {
                if (input.data.length <= 1) {
                    input.setData(Code.getCodes(input.id));
                }
            }
            this[input.id] = input;
        }

        if (this.option.data) {
            this.setData(this.option.data, this.option.callback);
        }
    }

    init();
    //console.log(this);
};

//---------------------------Grid---------------------------------------------//
var Grid = function (view, option) {
    this.name = view.name;
    this.option = option;

    var _grid = null;
    var _this = this;
    var idField = option.idField;
    this.query = null;

    //public
    this.search = function (callback) {
        this._queryData(false, callback);
    };

    this.load = function (callback) {
        this._queryData(true, callback);
    };

    this.validate = function (tabsId, tabIndex) {
        _grid.validate();
        if (_grid.isValid())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }

        var error = _grid.getCellErrors()[0];
        _grid.beginEditCell(error.record, error.column);
        return false;
    };

    this.getChanges = function (encode) {
        var data = _grid.datagrid('getChanges');
        return encode ? mini.encode(data) : data;
    };

    this.getSelecteds = function (encode) {
        var data = _grid.datagrid('getSelections');
        return encode ? JSON.stringify(data) : data;
    };

    this.getLength = function () {
        return this.getData().length;
    };

    this.getData = function (encode) {
        var data = _grid.datagrid('getData');
        return encode ? JSON.stringify(data) : data;
    };

    this.setData = function (data, callback) {
        this.clear();
        if (data) {
            _grid.datagrid('loadData', data);
            callback && callback({ sender: this, data: data });
        }
    };

    this.clear = function () {
        _grid.datagrid('loadData', []);
    };

    this.addRow = function (data, index) {
        if (!index) {
            index = this.getLength();
        }

        _grid.datagrid('insertRow', { index: index, row: data });
        _grid.datagrid('beginEdit', index);
    };

    this.updateRow = function (data, index) {
        _grid.datagrid('updateRow', { index: index, row: data });
    };

    this.deleteRow = function (index) {
        _grid.datagrid('deleteRow', index);
    };

    this.checkSelect = function (callback) {
        var rows = this.getSelecteds();
        if (rows.length === 0) {
            Message.tip('请选择一条记录！');
        } else if (rows.length > 1) {
            Message.tip('只能选择一条记录！');
        } else if (callback) {
            callback({ sender: this, row: rows[0] });
        }
    };

    this.checkMultiSelect = function (callback) {
        var rows = this.getSelecteds();
        if (rows.length === 0) {
            Message.tip('请选择一条或多条记录！');
        } else if (callback) {
            var data = this.getRowDatas(rows, null);
            callback({ sender: this, rows: rows, data: data });
        }
    };

    this.deleteRows = function (url, callback) {
        this.checkMultiSelect(function (e) {
            Message.confirm('确定要删除选中的' + e.rows.length + '记录？', function () {
                Ajax.action('删除', url, {
                    data: JSON.stringify(e.data)
                }, function(data) {
                    callback && callback({ sender: this, data: data });
                });
            });
        });
    };

    this.getRowDatas = function (rows, fields, encode) {
        var datas = [];
        if (fields) {
            $(rows).each(function (i, d) {
                var data = {};
                $(fields).each(function (i, p) {
                    data[p] = d[p] || '';
                });
                datas.push(data);
            });
        } else {
            $(rows).each(function (i, d) {
                datas.push(d[idField] || '');
            });
        }
        return encode ? JSON.stringify(datas) : datas;
    };

    this.hideColumn = function (field) {
        _grid.datagrid('hideColumn', field);
    };

    this.showColumn = function (field) {
        _grid.datagrid('showColumn', field);
    };

    //private
    function onColumnRende(e) {
        var displayField = e.column.displayField;
        if (displayField === 'icon') {
            var value = e.record[e.column.field];
            return '<span class="mini-icon mini-iconfont ' + e.value + '"></span>';
        } else if (displayField.startWith('code.')) {
            var type = displayField.replace('code.', '');
            var code = Code.getCode(type, e.value);
            var text = e.value;
            if (code && code.text) {
                text += '-' + code.text;
            }
            return text;
        } else {
            return e.record[displayField];
        }
    }

    function queryData(isLoad, callback) {
        var query = this.query ? this.query.getData(true) : '';
        _grid.clearSelect(false);
        _grid.load(
            { query: query, isLoad: isLoad },
            function (e) {
                if (callback) {
                    callback({ sender: this, result: e.result });
                }
            },
            function () {
                Message.tips({ content: '查询出错！', state: 'warning' });
            }
        );
        new ColumnsMenu(_grid);
    }

    function initQuery() {
        if ($('#query' + name).length) {
            this.query = new Form('query' + name);
            this.query.setData(this.option.query);

            if (this.query.key) {
                this.query.key.on('buttonclick', function () {
                    _this.search();
                });
            }

            var btnSearch = mini.get('search', this.query);
            if (btnSearch) {
                btnSearch.on('click', function () {
                    _this.search();
                });
            }
        }
    }

    function init() {
        //initQuery();

        var options = $.extend({
            bodyCls: 'grid' + _this.name,
            rownumbers: true, pagination: true, fit: true,
            fitColumns: true, striped: true, toolbar: []
        }, option);

        if (option.toolbars) {
            for (var i = 0; i < option.toolbars.length; i++) {
                var btn = Toolbar.find(option.toolbars[i]);
                if (btn) {
                    options.toolbar.push(btn);
                }
            }
        }
        _grid = $('#grid' + _this.name).datagrid(options);
        Toolbar.bind('.' + options.bodyCls + ' .datagrid-toolbar', view);
    }

    init();
    //console.log(this);
};

//---------------------------init---------------------------------------------//
$(function () {
    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });

    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });
});
