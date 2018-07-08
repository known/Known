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
    _request: function (type, dataType, url, param, callback) {
        var data;
        if (typeof param === 'function') {
            callback = param;
        } else {
            data = param;
        }

        if (url.startWith('/api/')) {
            data = { url: url, param: param };
            url = type === 'get' ? '/api/get' : '/api/post';
        }
        
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
    getText: function (url, param, callback) {
        this._request('get', 'text', url, param, callback);
    },
    postText: function (url, param, callback) {
        this._request('post', 'text', url, param, callback);
    },
    getJson: function (url, param, callback) {
        this._request('get', 'json', url, param, callback);
    },
    postJson: function (url, param, callback) {
        this._request('post', 'json', url, param, callback);
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
        if (event.which == 13) {
            event.preventDefault();
            var method = $(this).attr("onenter");
            eval(method);
        }
    });
});