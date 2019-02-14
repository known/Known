//---------------------------string------------------------------------------//
String.prototype.trim = function () {
    return this.replace(/(^\s*)|(\s*$)/g, '');
};

String.prototype.startWith = function (str) {
    var reg = new RegExp("^" + str);
    return reg.test(this);
};

String.prototype.endWith = function (str) {
    var reg = new RegExp(str + "$");
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

//---------------------------array------------------------------------------//
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

//---------------------------date------------------------------------------//
Date.prototype.format = function (fmt) {
    var o = {
        "M+": this.getMonth() + 1,               //月份   
        "d+": this.getDate(),                    //日   
        "h+": this.getHours(),                   //小时  
        "H+": this.getHours(),
        "m+": this.getMinutes(),                 //分   
        "s+": this.getSeconds(),                 //秒   
        "q+": Math.floor((this.getMonth() + 3) / 3), //季度   
        "S": this.getMilliseconds()              //毫秒   
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + "").substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp("(" + k + ")").test(fmt)) {
            var value = RegExp.$1.length === 1
                ? o[k]
                : ("00" + o[k]).substr(("" + o[k]).length);
            fmt = fmt.replace(RegExp.$1, value);
        }
    }
    return fmt;
};

Date.prototype.addYears = function (number) {
    var year = parseInt(this.getFullYear()) + number;
    return new Date(year, this.getMonth(), this.getDate(), this.getHours(), this.getMinutes(), this.getSeconds());
};

//---------------------------ajax------------------------------------------//
$(document).ajaxSend(function (evt, xhr, settings) {
    if (settings.type === 'POST') {
        var token = $('input[name="__RequestVerificationToken"]').val();
        xhr.setRequestHeader('X-XSRF-TOKEN', token);
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
    }

};

//---------------------------jquery------------------------------------------//
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
            Ajax.getText(url, param, function (result) {
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
    }

});

//---------------------------message------------------------------------------//
var Message = {

    loading: function (message, callback) {
        mini.mask({
            el: document.body,
            cls: 'mini-mask-loading',
            html: message
        });
        if (callback && callback()) {
            mini.unmask(document.body);
        }
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
            if (action === "ok") {
                callback && callback(value);
            }
        });
    },

    promptMulti: function (label, title, callback) {
        mini.prompt(label, title, function (action, value) {
            if (action === "ok") {
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
            title: option.title || "提示",
            iconCls: option.iconCls || "mini-messagebox-warning",
            message: option.message,
            timeout: option.timeout || 3000,
            x: option.x || 'right',
            y: option.y || 'bottom'
        });
    },

    result: function (res, callback) {
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

//---------------------------dialog------------------------------------------//
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
        dialog.setWidth(option.width || 500);
        dialog.setHeight(option.height || 300);
        dialog.show();

        if (option.max) {
            dialog.max();
        }

        $('#dialog .mini-panel-body').loadHtml(
            option.url,
            option.param,
            function () {
                mini.parse();
                option.callback && option.callback(dialog);
            }
        );

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

//---------------------------code------------------------------------------//
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