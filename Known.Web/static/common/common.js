//---------------------------array--------------------------------------------//
Array.prototype.insert = function (index, item) {
    this.splice(index, 0, item);
};

Array.prototype.contains = function (item) {
    for (i in this) {
        if (this[i] === item) {
            return true;
        }
    }
    return false;
};

Array.prototype.min = function (prop) {
    var min = prop ? this[0][prop] : this[0];
    var len = this.length;
    for (var i = 1; i < len; i++) {
        var item = prop ? this[i][prop] : this[i];
        if (item < min) {
            min = item;
        }
    }
    return min;
};

Array.prototype.max = function (prop) {
    var max = prop ? this[0][prop] : this[0];
    var len = this.length;
    for (var i = 1; i < len; i++) {
        var item = prop ? this[i][prop] : this[i];
        if (item > max) {
            max = item;
        }
    }
    return max;
};

//---------------------------date---------------------------------------------//
Date.prototype.format = function (fmt) {
    var o = {
        'M+': this.getMonth() + 1,               //月份   
        'd+': this.getDate(),                    //日   
        'h+': this.getHours(),                   //小时  
        'H+': this.getHours(),
        'm+': this.getMinutes(),                 //分   
        's+': this.getSeconds(),                 //秒   
        'q+': Math.floor((this.getMonth() + 3) / 3), //季度   
        'S': this.getMilliseconds()              //毫秒   
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp('(' + k + ')').test(fmt)) {
            var value = RegExp.$1.length === 1
                ? o[k]
                : ('00' + o[k]).substr(('' + o[k]).length);
            fmt = fmt.replace(RegExp.$1, value);
        }
    }
    return fmt;
};

Date.prototype.addYears = function (number) {
    var year = parseInt(this.getFullYear()) + number;
    return new Date(year, this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds());
};

//---------------------------string-------------------------------------------//
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, '');
};

String.prototype.startWith = function (str) {
    var reg = new RegExp('^' + str);
    return reg.test(this);
};

String.prototype.endWith = function (str) {
    var reg = new RegExp(str + '$');
    return reg.test(this);
};

String.prototype.padLeft = function (char, length) {
    var len = this.length;
    if (len === 0) return '';
    var str = this;
    while (len < length) {
        str = char + str;
        len++;
    }
    return str;
};

String.prototype.htmlEncode = function () {
    var div = document.createElement('div');
    div.appendChild(document.createTextNode(this));
    var html = div.innerHTML;
    html = html.replace(/\r\n/g, '<br/>');
    html = html.replace(/\n/g, '<br/>');
    return html;
};

String.prototype.htmlDecode = function () {
    var div = document.createElement('div');
    div.innerHTML = this;
    return div.innerText || div.textContent;
};

String.prototype.decode = function () {
    if (!arguments.length)
        return this;

    var result = arguments.length % 2 !== 0 ? arguments[arguments.length - 1] : '';
    for (var i = 0; i < arguments.length; i++) {
        if (this === arguments[i]) {
            result = arguments[i + 1];
            i++;
            break;
        }
    }

    return result;
};

//---------------------------jquery-------------------------------------------//
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

//---------------------------ajax---------------------------------------------//
$(document).ajaxSend(function (event, xhr, settings) {
    var user = User.getUser();
    if (user) {
        xhr.setRequestHeader('Authorization', 'Token ' + user.Token);
    }
});
$(document).ajaxError(function (event, xhr, settings, exception) {
    if (xhr.status === 401) {
        top.location = '/login.html';
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
        Ajax.postJson(url, { '': JSON.stringify(data) }, function (result) {
            Message.result(result, function (d) {
                callback && callback(d);
            });
        });
    }

};

//---------------------------code---------------------------------------------//
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

//---------------------------toolbar------------------------------------------//
var Toolbar = {

    bind: function (tbId, obj) {
        for (var p in obj) {
            bindButton(tbId, p, obj);
        }

        var top = !tbId.startsWith('tbForm');
        var btnClose = $('#' + tbId + ' #close');
        if (btnClose.length) {
            btnClose.unbind('click').bind('click', function () {
                Dialog.close(top);
            });
        }

        function bindButton(tbId, name, obj) {
            var btn = $('#' + tbId + ' #' + name);
            if (btn.length) {
                btn.unbind('click').bind('click', function () {
                    obj[name].call(obj);
                });
            }
        }
    }

};

//---------------------------url----------------------------------------------//
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

//---------------------------user---------------------------------------------//
var User = {

    key: 'known_user',

    setUser: function (user) {
        sessionStorage.setItem(this.key, JSON.stringify(user));
    },

    getUser: function () {
        return JSON.parse(sessionStorage.getItem(this.key));
    }

};

//---------------------------message------------------------------------------//
var Message1 = {

    mask: function (message, el) {
        mini.mask({
            el: el || document.body,
            cls: 'mini-mask-loading',
            html: message
        });
    },

    unmask: function (el) {
        mini.unmask(el || document.body);
    },

    alert: function (message, callback) {
        message = message.htmlEncode();
        mini.alert(message, '提示', function (action) {
            if (action === 'ok') {
                callback && callback();
            }
        });
    },

    error: function (message, callback) {
        mini.showMessageBox({
            title: '错误',
            message: '<span style="padding-left:10px;">' + message + '</span>',
            buttons: ['ok'],
            iconCls: 'mini-messagebox-error',
            callback: function (action) {
                if (action === 'ok') {
                    callback && callback();
                }
            }
        });
    },

    confirm: function (message, callback) {
        message = message.htmlEncode();
        mini.confirm(message, '确认提示', function (action) {
            if (action === 'ok') {
                callback && callback();
            }
        });
    },

    prompt: function (label, title, callback) {
        mini.prompt(label, title, function (action, value) {
            if (action === 'ok') {
                callback && callback(value);
            }
        });
    },

    promptMulti: function (label, title, callback) {
        mini.prompt(label, title, function (action, value) {
            if (action === 'ok') {
                callback && callback(value);
            }
        }, true);
    },

    tips: function (option) {
        if (typeof option === 'string')
            option = { content: option };

        mini.showTips({
            content: option.content,
            state: option.state || 'info',
            x: option.x || 'center',
            y: option.y || 'top',
            timeout: option.timeout || 3000
        });
    },

    notify: function (option) {
        mini.showMessageBox({
            showModal: false,
            width: option.width || 250,
            title: option.title || '提示',
            iconCls: option.iconCls || 'mini-messagebox-warning',
            message: option.message,
            timeout: option.timeout || 3000,
            x: option.x || 'right',
            y: option.y || 'bottom'
        });
    },

    result: function (res, callback) {
        this.unmask();

        if (!res.ok) {
            this.alert(res.message);
            return;
        }

        if (res.message) {
            this.tips(res.message);
        }

        callback && callback(res.data);
    }

};
var Message = {

    show: function () {
        $.messager.show({
            title: 'My Title',
            msg: 'Message will be closed after 4 seconds.',
            showType: 'show'
        });

        $.messager.show({
            title: 'My Title',
            msg: 'Message will be closed after 5 seconds.',
            timeout: 5000,
            showType: 'slide'
        });

        $.messager.show({
            title: 'My Title',
            msg: 'Message never be closed.',
            timeout: 0,
            showType: 'fade'
        });
    },

    alert: function (message) {
        $.messager.alert('提示', message, 'info');
    },

    confirm: function (message, callback) {
        $.messager.confirm('确认提示', message, function (r) {
            callback && callback(r);
        });
    },

    prompt: function () {
        $.messager.prompt('提示', 'Please enter your name:', function (r) {
            if (r) {
                alert('Your name is:' + r);
            }
        });
    },

    progress: function () {
        $.messager.progress({
            title: 'Please waiting',
            msg: 'Loading data...'
        });
        setTimeout(function () {
            $.messager.progress('close');
        }, 5000);
    }

};

//---------------------------dialog-------------------------------------------//
var Dialog = {

    show: function (option) {
        if (option.id) {
            var win = mini.get(option.id);
            win.show();
            option.callback && option.callback(win);
            return win;
        }

        var dialog = mini.get('dialog');
        dialog.setTitle(option.title);
        dialog.setIconCls(option.iconCls || 'fa-windows');
        dialog.setWidth(option.width || 500);
        dialog.setHeight(option.height || 300);
        dialog.show();

        if (option.max) {
            dialog.max();
        }

        if (option.url) {
            $('#dialog .mini-panel-body').loadHtml(
                option.url,
                option.param,
                function () {
                    mini.parse();
                    option.callback && option.callback(dialog);
                }
            );
        }

        return dialog;
    },

    open: function (option) {
        var win = mini.open({
            url: option.url,
            showMaxButton: true,
            allowResize: true,
            title: option.title,
            width: option.width,
            height: option.height,
            onload: function () {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, 'load');
                }
            },
            ondestroy: function (action) {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, action);
                }
            }
        });
        option.max && win.max();
        return win;
    },

    close: function (top = false) {
        if (top) {
            window.CloseOwnerWindow();
        } else {
            mini.get('dialog').hide();
        }
    }

};

//---------------------------form---------------------------------------------//
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
    this._init = function () {
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
    };

    this._init();
    //console.log(this);
};

//---------------------------grid---------------------------------------------//
var Grid = function (name, option) {
    this.name = name;
    this.option = option;

    var _grid = mini.get('grid' + name);
    $.extend(true, this, _grid);

    var _this = this;
    this.idField = _grid.getIdField();
    this.query = null;

    //public
    this.on = function (type, fn) {
        _grid.on(type, fn);
    };

    this.un = function (type, fn) {
        _grid.un(type, fn);
    };

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
        var data = _grid.getChanges();
        return encode ? mini.encode(data) : data;
    };

    this.getSelecteds = function (encode) {
        _grid.accept();
        var data = _grid.getSelecteds();
        return encode ? mini.encode(data) : data;
    };

    this.getLength = function () {
        return _grid.getData().length;
    };

    this.getData = function (encode) {
        var data = _grid.getData();
        return encode ? mini.encode(data) : data;
    };

    this.setData = function (data, callback) {
        this.clear();
        if (data) {
            _grid.setData(data);
            callback && callback(data);
        }
    };

    this.clear = function () {
        _grid.setData([]);
    };

    this.addRow = function (data, index) {
        if (!index) {
            index = _grid.getData().length;
        }

        _grid.addRow(data, index);
        _grid.cancelEdit();
        _grid.beginEditRow(data);
    };

    //this.updateRow = function (e, data) {
    //    e.sender.updateRow(e.record, data);
    //};

    this.deleteRow = function (uid) {
        var row = _grid.getRowByUid(uid);
        if (row) {
            _grid.removeRow(row);
        }
    };

    this.checkSelect = function (callback) {
        var rows = _grid.getSelecteds();
        if (rows.length === 0) {
            Message.tips('请选择一条记录！');
        } else if (rows.length > 1) {
            Message.tips('只能选择一条记录！');
        } else if (callback) {
            callback(rows[0]);
        }
    };

    this.checkMultiSelect = function (callback) {
        var rows = _grid.getSelecteds();
        if (rows.length === 0) {
            Message.tips('请选择一条或多条记录！');
        } else if (callback) {
            var data = this.getRowDatas(rows, null);
            callback(rows, data);
        }
    };

    this.deleteRows = function (url, callback) {
        this.checkMultiSelect(function (rows, data) {
            Message.confirm('确定要删除选中的记录？', function () {
                Ajax.action('删除', url, data, callback);
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
            var id = _grid.idField;
            $(rows).each(function (i, d) {
                datas.push(d[id] || '');
            });
        }
        return encode ? mini.encode(datas) : datas;
    };

    this.hideColumn = function (indexOrName) {
        var column = _grid.getColumn(indexOrName);
        _grid.updateColumn(column, { visible: false });
    };

    this.showColumn = function (indexOrName) {
        var column = _grid.getColumn(indexOrName);
        _grid.updateColumn(column, { visible: true });
    };

    //private
    this._onColumnRender = function (e) {
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
    };

    this._queryData = function (isLoad, callback) {
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
    };

    this._initQuery = function () {
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
    };

    this._init = function () {
        this._initQuery();

        _grid.set(this.option);

        var columns = _grid.getColumns();
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].displayField) {
                _grid.updateColumn(columns[i], {
                    renderer: _this._onColumnRender
                });
            }
        }
    };

    this._init();
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
