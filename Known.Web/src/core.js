/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-26     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

const DateFormat = 'yyyy-MM-dd';
const DateTimeFormat = 'yyyy-MM-dd HH:mm:ss';
const KeyToken = 'Known-Token';
const KeyUser = 'Known_User';
const KeyCode = 'Known_Codes';
const KeyClient = 'Known_Client';
const KeyDownload = 'Known-Download';

Date.prototype.format = function (fmt) {
    var o = {
        'M+': this.getMonth() + 1,//月份
        'd+': this.getDate(),//日
        'h+': this.getHours(),//小时
        'H+': this.getHours(),
        'm+': this.getMinutes(),//分
        's+': this.getSeconds(),//秒
        'q+': Math.floor((this.getMonth() + 3) / 3), //季度
        'S': this.getMilliseconds()//毫秒
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp('(' + k + ')').test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1)
                ? o[k]
                : ('00' + o[k]).substr(('' + o[k]).length));
        }
    }
    return fmt;
};

function log(obj) { console.log(obj); }

function Router(elem, option) {
    //fields
    var _option = option || {},
        _elem = elem,
        _current = {},
        _this = this;

    //properties
    this.elem = _elem;

    //methods
    this.route = function (item) {
        var currComp = _current.component;
        currComp && currComp.destroy && currComp.destroy();

        if (!item.previous) {
            item.previous = _current;
        }
        _current = item;

        var component = item.component;
        if (component) {
            if (typeof component === 'string') {
                _elem.html(component);
            } else {
                _elem.html('');
                component.render(_elem);
            }
            setTimeout(function () {
                component.mounted && component.mounted();
                Page.complete();
            }, 10);
        }
    }

    this.back = function () {
        _this.route(_current.previous);
    }

}

function WsUtil() {
    var ws = null;
    var isClosed = false;
    var _this = this;

    this.connectById = function (id, data, callback) {
        window[id] = {};
        this.connect('/ws?id=' + id, data, function (res) {
            if (res && res.length) {
                var obj = JSON.parse(res);
                callback && callback(obj);
            }
        });
    }

    this.connect = function (url, data, callback) {
        url = WsUtil.getUrl(url, data);
        log('Connecting......');
        ws = new WebSocket(url);
        ws.onopen = function () {
            log('Connected');
        }
        ws.onmessage = function (evt) {
            if (evt.data) {
                callback && callback(evt.data);
            }
        }
        ws.onclose = function () {
            if (!isClosed) {
                log('Disconnected');
                _this.connect(url, data, callback);
            }
        }
        ws.onerror = function (evt) {
            log(evt);
            _this.close();
        }
        window.onbeforeunload = function () {
            _this.close();
        }
        return ws;
    }

    this.send = function (message) {
        if (ws) {
            ws.send(message);
        }
    }

    this.close = function () {
        isClosed = true;
        if (ws) {
            ws.close();
            ws = null;
            log('Closed');
        }
    }
}

WsUtil.getUrl = function (url, data) {
    if (!(url.indexOf('ws://') > -1 || url.indexOf('wss://') > -1)) {
        var base = location.protocol === 'https:' ? 'wss://' : 'ws://';
        if (url.indexOf(location.host) < 0) {
            base += location.host;
        }
        url = base + url;
    }

    if (data) {
        url += url.indexOf('?') < 0 ? '?' : '&';
        var nvs = [];
        for (var p in data) {
            nvs.push(p + '=' + data[p])
        }
        url += nvs.join('&');
    }

    return url;
}

var Utils = {

    getUrlParam: function (name) {
        var reg = new RegExp('(^|&)' + name + '=([^&]*)(&|$)', 'i');
        var res = window.location.search.substr(1).match(reg);
        if (res == null)
            return '';

        return decodeURI(res[2]);
    },

    getClientId: function () {
        var binToHex = function (str) {
            var res = '';
            for (i = 0; i < str.length; i++) {
                res += intToHex(str.charCodeAt(i));
            }
            return res;
        };
        var intToHex = function (i) {
            var res = i.toString(16);
            var j = 0;
            while (j + res.length < 4) {
                res = '0' + res;
                j++;
            }
            return res;
        };
        var canvas = document.createElement('canvas');
        var ctx = canvas.getContext('2d');
        ctx.font = '14px Arial';
        ctx.fillStyle = '#ccc';
        ctx.fillText(KeyClient, 2, 2);
        var fp = canvas.toDataURL().replace('data:image/png;base64,', '');
        var bin = window.atob(fp);
        var crc = binToHex(bin.slice(-16, -12));
        return crc;
    },

    loadJs: function (src) {
        window.onload = function () {
            var script = document.createElement('script');
            script.src = src;
            document.body.appendChild(script);
        }
    },

    format: function (source, params) {
        if (arguments.length == 1) {
            return function () {
                var args = $.makeArray(arguments);
                args.unshift(source);
                return $.format.apply(this, args);
            };
        }

        if (arguments.length > 2 && params.constructor != Array) {
            params = $.makeArray(arguments).slice(1);
        }
        if (params.constructor != Array) {
            params = [params];
        }
        $.each(params, function (i, n) {
            source = source.replace(new RegExp('\\{' + i + '\\}', 'g'), n);
        });
        return source;
    },

    copy: function (text) {
        var el = document.createElement('textarea');
        el.value = text;
        el.style.position = 'absolute';
        el.style.left = '-9999px';
        document.body.appendChild(el);
        var selection = document.getSelection();
        var selected = selection.rangeCount > 0 ? selection.getRangeAt(0) : false;
        el.select();
        document.execCommand('copy');
        document.body.removeChild(el);
        if (selected) {
            selection.removeAllRanges();
            selection.addRange(selected);
        }
    },

    paste: function (enabled, callback) {
        if (enabled) {
            $(document).bind('paste', function () {
                if (event.clipboardData || event.originalEvent) {
                    var clipboardData = (event.clipboardData || window.clipboardData);
                    var text = clipboardData.getData('text');
                    callback && callback(text);
                    event.preventDefault();
                }
            });
        } else {
            $(document).unbind('paste');
        }
    },

    list2Tree: function (data, pid) {
        if (!data) return [];
        let list = data.slice();
        return list.filter(function (parent) {
            let branchArr = list.filter(function (child) {
                return parent.id === child.pid;
            });
            if (branchArr.length > 0) {
                parent.children = branchArr;
            }
            return parent.pid === pid;
        });
    },

    tree2List: function (data) {
        if (!data) return [];

        let list = [];
        let queue = data.slice();
        while (queue.length) {
            let node = queue.shift();
            list.push(node);
            let children = node.children;
            if (children && children.length) {
                for (let i = 0; i < children.length; i++)
                    queue.push(children[i]);
            }
        }
        return list;
        //var tree = data.slice();
        //return tree.reduce(function (con, item) {
        //    var callee = arguments.callee;
        //    con.push(item);
        //    if (item[key] && item[key].length > 0)
        //        item[key].reduce(callee, con);
        //    return con;
        //}, []).map(function (item) {
        //    item[key] = [];
        //    return item;
        //});
    },

    getExtIcon: function (ext) {
        switch (ext) {
            case '.pdf':
                return 'fa fa-file-pdf-o pdf';
            case '.doc':
            case '.docx':
                return 'fa fa-file-word-o doc';
            case '.xls':
            case '.xlsx':
                return 'fa fa-file-excel-o xls';
            case '.ppt':
            case '.pptx':
                return 'fa fa-file-powerpoint-o ppt';
            case '.jpg':
            case '.jpeg':
            case '.gif':
            case '.png':
            case '.tif':
                return 'fa fa-file-photo-o';
            case '.rar':
            case '.zip':
            case '.gz':
                return 'fa fa-file-zip-o';
            default:
                return 'fa fa-file-o';
        }
    },

    setUser: function (user) {
        var key = KeyUser;
        if (!user) {
            sessionStorage.removeItem(key);
        } else {
            sessionStorage.setItem(key, JSON.stringify(user));
        }
    },

    getUser: function () {
        var value = sessionStorage.getItem(KeyUser);
        if (!value || value === 'undefined')
            return null;

        return JSON.parse(value);
    },

    setCodes: function (data) {
        var key = KeyCode;
        if (!data) {
            localStorage.removeItem(key);
        } else {
            localStorage.setItem(key, JSON.stringify(data));
        }
    },

    getCodes: function (data) {
        if (!data)
            return [];

        if ($.isArray(data))
            return data;

        if ($.isPlainObject(data)) {
            var datas = [];
            for (var p in data) {
                datas.push({ Code: p, Name: p, Data: data[p] });
            }
            return datas;
        }

        var value = localStorage.getItem(KeyCode);
        if (!value)
            return [];

        var codes = JSON.parse(value);
        if (!codes)
            return [];

        return codes[data] || [];
    },

    getCodeName: function (data, value) {
        var codes = this.getCodes(data);
        if (codes && codes.length) {
            if (typeof value === 'string' && value.indexOf(',') > 0) {
                var values = value.split(',');
                var names = [];
                for (var i = 0; i < values.length; i++) {
                    var code = codes.filter(function (c) { return c.Code === values[i]; });
                    if (code.length) {
                        names.push(code[0].Name || value);
                    }
                }
                return names.join(',');
            } else {
                var code = codes.filter(function (c) { return c.Code === value; });
                if (code.length) {
                    return code[0].Name || value;
                }
            }
        }

        return value;
    },

    getCodeObject: function (category, nameArray) {
        var obj = {};
        var codes = this.getCodes(category);
        if (codes && codes.length) {
            for (var i = 0; i < codes.length; i++) {
                var item = codes[i];
                if (nameArray) {
                    nameArray.push(item.Name);
                }
                obj[item.Code] = item.Name;
            }
        }

        return obj;
    },

    genFile: function (text, saveName) {
        var blob = new Blob([s2ab(text)], { type: 'application/octet-stream' });
        //if (typeof url == 'object' && url instanceof Blob) {
        //    url = URL.createObjectURL(blob);
        //}
        var url = URL.createObjectURL(blob);
        var aLink = document.createElement('a');
        aLink.href = url;
        aLink.download = saveName || ''; // HTML5新增的属性，指定保存文件名，可以不要后缀，注意，file:///模式下不会生效
        var event;
        if (window.MouseEvent) {
            event = new MouseEvent('click');
        } else {
            event = document.createEvent('MouseEvents');
            event.initMouseEvent('click', true, false, window, 0, 0, 0, 0, 0, false, false, false, false, 0, null);
        }
        aLink.dispatchEvent(event);

        // String to ArrayBuffer
        function s2ab(s) {
            var buf = new ArrayBuffer(s.length);
            var view = new Uint8Array(buf);
            for (var i = 0; i != s.length; ++i)
                view[i] = s.charCodeAt(i) & 0xFF;
            return buf;
        }
    },

    checkMobile: function () {
        var agent = navigator.userAgent;
        var agents = new Array('Android', 'iPhone', 'SymbianOS', 'Windows Phone', 'iPad', 'iPod');
        var flag = false;
        for (var i = 0; i < agents.length; i++) {
            if (agent.indexOf(agents[i]) > 0) {
                flag = true;
                break;
            }
        }
        return flag;
    },

    printIframe: null,
    print: function (option) {
        var iframe = this.printIframe;
        if (!this.printIframe) {
            iframe = this.printIframe = document.createElement('iframe');
            document.body.appendChild(iframe);
            iframe.style.display = 'none';
        }
        if (option.url) {
            iframe.src = option.url;
            if (!option.ajax) {
                iframe.onload = function () {
                    setTimeout(function () { iframe.contentWindow.print(); }, 1);
                };
            }
        } else if (option.html) {
            var doc = iframe.contentWindow.document;
            doc.open();
            doc.write('<title>' + option.title + '</title>');
            $(document).find('link').filter(function () {
                return $(this).attr('rel').toLowerCase() === 'stylesheet';
            }).each(function () {
                doc.write('<link rel="stylesheet" href="' + $(this).attr('href') + '">');
            });
            doc.write('<style>html,body{overflow:auto;height:auto;}.form-table{padding:0;}</style>');

            setTimeout(function () {
                if (typeof option.html === 'function') {
                    doc.write(option.html());
                } else {
                    doc.write(option.html);
                }
                setTimeout(function () { iframe.contentWindow.print(); }, 1);
            }, 100);
        }
    },

    initPCAField: function (e) {
        if (!window.PCARegion)
            return;

        e.form.Province.setData(window.PCARegion, e.data.Province);

        e.form.Province.change(function (e) {
            e.form.City.setData(e.selected.Data);
            if (e.form.Area) {
                e.form.Area.setData([]);
            }
        });

        if (e.data.Province) {
            var selProv = e.form.Province.getData() || {};
            e.form.City.setData(selProv.Data, e.data.City);
        }

        if (e.form.Area) {
            e.form.City.change(function (e) {
                e.form.Area.setData(e.selected.Data);
            });

            if (e.data.City) {
                var selCity = e.form.City.getData() || {};
                e.form.Area.setData(selCity.Data, e.data.Area);
            }
        }
    },

    createButton: function (item, arg) {
        var button = $('<button>').data('item', item);

        if (item.icon) {
            $('<i>').addClass(item.icon).appendTo(button);
        }

        if (item.style) {
            button.addClass(item.style);
        }

        $('<span>').html(item.text || item.name).appendTo(button);

        button.on('click', function () {
            var itm = $(this).data('item');
            if (itm.handler) {
                if (arg && $.isFunction(arg)) {
                    arg = arg();
                }
                var e = $.extend({ item: itm }, arg);
                itm.handler.call(this, e);
            }
        });

        return button;
    },

    parseDom: function (dom, template, arg) {
        if (typeof template === 'function') {
            template(arg);
        } else {
            dom.append(template);
        }
    }

};

var Layer = {
    open: function (option) { },
    loading: function (message) { },
    tips: function (message) { },
    alert: function (message, callback) { },
    confirm: function (message, callback) { }
};

var Ajax = {

    get: function (url, data, callback) {
        $.get(url, data, function (result) {
            if (result.Message) {
                Layer.tips(result.Message);
            }
            if (result.IsValid) {
                callback && callback(result.Data);
            } else {
                callback && callback(result);
            }
        });
    },

    post: function (url, data, callback) {
        var dlg = Layer.loading();
        $.post(url, data, function (result) {
            dlg.close();
            if (result.IsValid) {
                Layer.tips(result.Message);
                callback && callback(result.Data);
            } else if (result.Message) {
                Layer.alert(result.Message);
            } else {
                callback && callback(result);
            }
        });
    },

    upload: function (fileId, url, data, callback) {
        var dlg = Layer.loading(Language.Uploading + '......');
        var fd = new FormData(), xhr = new XMLHttpRequest();
        var files = document.getElementById(fileId).files;

        for (var i = 0; i < files.length; i++) {
            fd.append('file' + i, files[i], files[i].name);
        }

        if (data) {
            fd.append('data', JSON.stringify(data));
        }

        xhr.onload = function () {
            dlg.close();
            var result = JSON.parse(xhr.responseText);
            if (result.IsValid) {
                Layer.tips(result.Message);
                callback && callback(result.Data);
            } else if (result.Message) {
                Layer.alert(result.Message);
            } else {
                callback && callback(result);
            }
        };

        xhr.upload.onprogress = function (evt) {
            var loaded = evt.loaded;
            var total = evt.total;
            //per = Math.floor(100 * loaded / total);
            //var progressBar = document.getElementById('progressBar');
            //var percentageDiv = document.getElementById('percentage');
            //if (evt.lengthComputable) {
                //progressBar.max = total;
                //progressBar.value = loaded;
                //percentageDiv.innerHTML = Math.round(loaded / total * 100) + '%';
            //}
        };

        xhr.open('post', url, true);
        xhr.send(fd);
    },

    download: function (url, data, callback) {
        var tokenKey = KeyDownload;
        var tokenValue = 'xxxxxxxx-xxxx-4xxx-yxxx-xxxxxxxxxxxx'.replace(/[xy]/g, function (c) {
            var r = Math.random() * 16 | 0, v = c === 'x' ? r : (r & 0x3 | 0x8);
            return v.toString(16);
        });

        function getCookie(name) {
            var reg = new RegExp('(^| )' + name + '=([^;]*)(;|$)');
            var arr = document.cookie.match(reg);
            if (arr && arr.length > 3)
                return unescape(arr[2]);
            return null;
        }

        function loading() {
            var dlg = Layer.loading(Language.Downloading + '......');
            var downloadTimer = window.setInterval(function () {
                var token = getCookie(tokenKey);
                if (token === tokenValue) {
                    window.clearInterval(downloadTimer);
                    dlg.close();
                    callback && callback();
                }
            }, 1000);
        }

        var form = $('<form>').attr('method', 'post').attr('action', url)
            .attr('target', '').appendTo('body').hide();
        $('<input>').attr('type', 'hidden').attr('name', tokenKey)
            .attr('value', tokenValue).appendTo(form);

        if (data && data !== null) {
            for (var p in data) {
                $('<input>').attr('type', 'hidden').attr('name', p)
                    .attr('value', data[p]).appendTo(form);
            }
        }

        form.submit();
        loading();
    }

};

var Page = {
    complete: function () {
        $('.dropdown .title').on('mouseenter', function () {
            $(this).find('.arrow').removeClass('fa-caret-down').addClass('fa-caret-up');
            $(this).next().show();
        });
        $('.dropdown').on('mouseleave', function () {
            var title = $(this).find('.title');
            title.find('.arrow').removeClass('fa-caret-up').addClass('fa-caret-down');
            title.next().hide();
        });

        // tab item change
        $('.tabs-header li').click(function () {
            var tab = $(this).parent().parent();
            var index = $(this).index();
            tab.find('.tabs-header li').removeClass('active');
            tab.find('.tabs-item').removeClass('active');
            $(this).addClass('active');
            tab.find('.tabs-item:eq(' + index + ')').addClass('active');
        });

        // bind input enter
        $('input[onenter]').keydown(function (event) {
            if ((event.keyCode || event.which) === 13) {
                event.preventDefault();
                var method = $(this).attr('onenter');
                eval(method);
            }
        });
    }
};

var curUser = Utils.getUser();
var app = new Router($('#app'));

$(document).ajaxSend(function (evt, xhr, settings) {
    xhr.setRequestHeader(KeyToken, curUser ? curUser.Token : '');
    xhr.setRequestHeader(KeyClient, Utils.getClientId());
});
$(document).ajaxComplete(function (evt, xhr, settings) {
    if (xhr.responseText && xhr.responseText.match('^\{(.+:.+,*){1,}\}$')) {
        var res = JSON.parse(xhr.responseText);
        if (res) {
            if (res.timeout) {
                this.location.reload();
            } else if (res.error) {
                app.route({ component: new Error(res) });
            }
        }
    }
});
$(document).contextmenu(function (e) { e.preventDefault(); });