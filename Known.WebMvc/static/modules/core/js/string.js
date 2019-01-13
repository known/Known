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