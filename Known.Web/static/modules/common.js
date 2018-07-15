///////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////
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

///////////////////////////////////////////////////////////////////////
var Ajax = {
    _request: function (type, dataType, args) {
        var url = args[0],
            data = null,
            param = null,
            callback = null;

        if (args.length > 2) {
            data = args[1];
            param = args[1];
            callback = args[2];
        } else if (args.length > 1) {
            if (typeof args[1] === 'function') {
                callback = args[1];
            } else {
                data = args[1];
                param = args[1];
            }
        }

        if (url.startWith('/api/')) {
            data = { url: url, param: param };
            url = type === 'get' ? '/api/get' : '/api/post';
        }

        //console.log(url);
        //console.log(data);
        //console.log(callback);
        
        $.ajax({
            type: type, dataType: dataType,
            url: url, data: data,
            cache: false, async: true,
            success: function (result) {
                if (callback) {
                    callback(result);
                }
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

///////////////////////////////////////////////////////////////////////
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
        mini.showTips({
            content: option.content,
            state: option.state || 'info',
            x: option.x || 'center',
            y: option.y || 'center',
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
        if (!res.IsValid) {
            this.alert(res.Message);
            return;
        }

        if (res.Message) {
            this.tips({
                message: res.Message, x: 'center', y:'top'
            });
        }

        callback && callback(res.Data);
    }
};

///////////////////////////////////////////////////////////////////////
var Dialog = {
    show: function (option) {
        var dialog = UI.get('dialog' + option.name);
        if (dialog) {
            option.callback && option.callback();
        } else {
            Ajax.getText(option.url, {
                name: option.name, model: option.model || null
            }, function (html) {
                $('body').append(html);
                mini.parse();
                option.callback && option.callback();
            });
        }
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
    }
};

///////////////////////////////////////////////////////////////////////
var Form = {
    get: function (form) {
        if (typeof form === 'string')
            return new mini.Form('#' + form);

        return form;
    },
    clear: function (form, controls) {
        if (controls) {
            $(controls.split(',')).each(function(i, c) {
                var control = mini.getbyName(c, form);
                if (control) {
                    control.setValue('');
                    if (control.type === 'autocomplete') {
                        control.setText('');
                    }
                }
            });
        } else {
            this.get(form).clear();
        }
    },
    validate: function (form, tabsId, tabIndex) {
        if (this.get(form).validate())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }
        return false;
    },
    getData: function (form, encode) {
        var data = this.get(form).getData(true);
        return encode ? mini.encode(data) : data;
    },
    setData: function (form, data, callback) {
        var ctl = this.get(form);
        if (ctl && data) {
            ctl.setData(data);
            callback && callback(data);
            ctl.setChanged(false);
        }
    },
    bindEnterJump: function (form) {
        var inputs = this.get(form).getFields();
        var activeIndexes = [];

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
                activeIndexes.push(i);
        }

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
                        var nextInput = inputs[nextIndex];
                        setTimeout(function () {
                            nextInput.focus();
                            if (nextInput.type !== 'textarea') {
                                nextInput.selectText();
                            }
                        }, 10);
                    } else if (i > 0 && e.keyCode === 38) {
                        var preInput = inputs[activeIndexes[i - 1]];
                        if (current.type !== 'textarea' && (
                            (current.type !== 'autocomplete' && current.type !== 'combobox')
                            || !current.isShowPopup()
                        )) {
                            setTimeout(function () {
                                preInput.focus();
                                if (preInput.type !== 'textarea') {
                                    preInput.selectText();
                                }
                            }, 10);
                        }
                    }
                });
            })(i);
        }
    },
    model: function (form, isLabel) {
        var labelClass = 'form-input-label-model';
        $('span.' + labelClass).remove();
        var inputs = this.get(form).getFields();
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
    }
};

///////////////////////////////////////////////////////////////////////
$(document).ajaxSend(function (evt, xhr, settings) {
    if (settings.type === 'POST') {
        var token = $('input[name="__RequestVerificationToken"]').val();
        xhr.setRequestHeader('X-XSRF-TOKEN', token);
    }
});

$(function () {
    $('input[onenter]').keydown(function (event) {
        if (event.which === 13) {
            event.preventDefault();
            var method = $(this).attr("onenter");
            eval(method);
        }
    });
});