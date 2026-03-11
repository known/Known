function findLink(match) {
    var items = document.getElementsByTagName('link');
    return Array.from(items).find((item) => item.getAttribute('href')?.match(match));
}

function createLink(href) {
    if (findLink(href)) return null;
    let item = document.createElement('link');
    item.rel = 'stylesheet';
    item.href = href;
    return item;
}

function setStyleSheet(match, href) {
    let item = findLink(match);
    if (!item) {
        item = createLink('');
        document.head.appendChild(item);
    }
    item.href = href;
}

function insertStyleSheet(match, href) {
    var item1 = createLink(href);
    if (item1) {
        let item = findLink(match);
        document.head.insertBefore(item1, item);
    }
}

function removeStyleSheet(href) {
    let item = findLink(href);
    if (item) {
        document.head.removeChild(item);
    }
}

const themeColors = {
    default: '#1890ff',
    dust: '#F5222D',
    volcano: '#FA541C',
    sunset: '#FAAD14',
    cyan: '#13C2C2',
    green: '#52C41A',
    geekblue: '#2F54EB',
    purple: '#722ED1'
};

function toRgb(hex) {
    if (!hex) return null;
    var value = hex.replace('#', '');
    if (value.length === 3)
        value = value.split('').map(x => x + x).join('');
    var num = parseInt(value, 16);
    return {
        r: (num >> 16) & 255,
        g: (num >> 8) & 255,
        b: num & 255
    };
}

function toHex(rgb) {
    const n = Math.max(0, Math.min(255, Math.round(rgb)));
    return n.toString(16).padStart(2, '0');
}

function mixColor(color, target, amount) {
    const from = toRgb(color);
    const to = toRgb(target);
    if (!from || !to) return color;
    const p = Math.max(0, Math.min(1, amount));
    return `#${toHex(from.r + (to.r - from.r) * p)}${toHex(from.g + (to.g - from.g) * p)}${toHex(from.b + (to.b - from.b) * p)}`;
}

function normalizeColor(color) {
    if (!color) return themeColors.default;
    return color.startsWith('#') ? color : (themeColors[color.toLowerCase()] || themeColors.default);
}

function setAntThemeColor(color) {
    const root = document.documentElement.style;
    const primary = normalizeColor(color);
    const rgb = toRgb(primary);

    root.setProperty('--kui-primary-color', primary);
    root.setProperty('--ant-primary-color', primary);
    root.setProperty('--ant-primary-color-hover', mixColor(primary, '#ffffff', 0.18));
    root.setProperty('--ant-primary-color-active', mixColor(primary, '#000000', 0.12));
    root.setProperty('--ant-primary-color-outline', `rgba(${rgb.r}, ${rgb.g}, ${rgb.b}, 0.2)`);
    root.setProperty('--ant-primary-1', mixColor(primary, '#ffffff', 0.9));
    root.setProperty('--ant-primary-2', mixColor(primary, '#ffffff', 0.75));
    root.setProperty('--ant-primary-3', mixColor(primary, '#ffffff', 0.58));
    root.setProperty('--ant-primary-4', mixColor(primary, '#ffffff', 0.42));
    root.setProperty('--ant-primary-5', mixColor(primary, '#ffffff', 0.2));
    root.setProperty('--ant-primary-6', primary);
    root.setProperty('--ant-primary-7', mixColor(primary, '#000000', 0.14));
    root.setProperty('--ant-info-color', primary);
    root.setProperty('--ant-info-color-deprecated-bg', mixColor(primary, '#ffffff', 0.9));
    root.setProperty('--ant-info-color-deprecated-border', mixColor(primary, '#ffffff', 0.58));
}

function setSizeMode(size) {
    const root = document.documentElement.style;
    if ((size || '').toLowerCase() === 'compact') {
        root.setProperty('--ant-font-size-base', '12px');
        root.setProperty('--ant-control-height', '28px');
        root.setProperty('--ant-control-height-lg', '32px');
        root.setProperty('--ant-control-height-sm', '22px');
    } else {
        root.removeProperty('--ant-font-size-base');
        root.removeProperty('--ant-control-height');
        root.removeProperty('--ant-control-height-lg');
        root.removeProperty('--ant-control-height-sm');
    }
}

function createCaptcha(canvas, code) {
    var ctx = canvas.getContext("2d");
    var width = ctx.canvas.width;
    var height = ctx.canvas.height;
    ctx.clearRect(0, 0, width, height);
    ctx.lineWidth = 2;
    for (var i = 0; i < 1000; i++) {
        ctx.beginPath();
        var x = getRandom(width - 2);
        var y = getRandom(height - 2);
        ctx.moveTo(x, y);
        ctx.lineTo(x + 1, y + 1);
        ctx.strokeStyle = getColor();
        ctx.stroke();
    }
    for (var i = 0; i < 20; i++) {
        ctx.beginPath();
        var x = getRandom(width - 2);
        var y = getRandom(height - 2);
        var w = getRandom(width - x);
        var h = getRandom(height - y);
        ctx.moveTo(x, y);
        ctx.lineTo(x + w, y + h);
        ctx.strokeStyle = getColor();
        ctx.stroke();
    }
    ctx.font = width / 5 + 'px 微软雅黑';
    ctx.textBaseline = 'middle';
    var codes = code.split('');
    for (var i = 0; i < codes.length; i++) {
        ctx.beginPath();
        ctx.fillStyle = '#f00';
        var word = codes[i];
        var w = width / codes.length;
        var left = getRandom(i * w, (i + 1) * w - width / 5);
        var top = getRandom(height / 2 - 10, height / 2 + 10);
        ctx.fillText(word, left, top);
    }

    function getRandom(a, b = 0) {
        var max = a; var min = b;
        if (a < b) { max = b; min = a; }
        return Math.floor(Math.random() * (max - min)) + min;
    }

    function getColor() {
        return `rgb(${Math.floor(Math.random() * 255)},${Math.floor(Math.random() * 256)},${Math.floor(Math.random() * 256)})`;
    }
}

export class KBlazor {
    //Callback
    static runScript(script) { return eval(script); }
    static runScriptVoid(script) { eval(script); }
    static invokeDotNet(id, key, param) {
        return DotNet.invokeMethodAsync('Known', 'CallbackByParamAsync', id, key, param);
    }
    //Common
    static elemClick(id) { document.getElementById(id).click(); }
    static elemEnabled(id, enabled) { document.getElementById(id).enabled = enabled; }
    //File
    static setLocalInfo(info) {
        var theme = info?.theme;
        if (!theme) {
            var hour = new Date().getHours();
            theme = hour > 6 && hour < 20 ? "light" : "dark";
        }
        $('html').attr('data-theme', theme);
        setAntThemeColor(info?.color);
        setSizeMode(info?.size);

        var darkUrl = '_content/AntDesign/css/ant-design-blazor.dark.css';
        if (theme == 'dark')
            insertStyleSheet('/Known/css/font-awesome.css', darkUrl);
        else
            removeStyleSheet(darkUrl);
    }
    //Storage
    static getLocalStorage(key) { return localStorage.getItem(key); }
    static setLocalStorage(key, value) {
        if (value)
            localStorage.setItem(key, value);
        else
            localStorage.removeItem(key);
    }
    static getSessionStorage(key) { return sessionStorage.getItem(key); }
    static setSessionStorage(key, value) {
        if (value)
            sessionStorage.setItem(key, value);
        else
            sessionStorage.removeItem(key);
    }
    //Screen
    static openFullScreen() {
        var el = document.documentElement;
        var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
        if (rfs) {
            rfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
    }
    static closeFullScreen() {
        var el = document;
        var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
        if (cfs) {
            cfs.call(el);
        } else if (typeof window.ActiveXObject !== 'undefined') {
            var wscript = new ActiveXObject('WScript.Shell');
            if (wscript !== null) {
                wscript.SendKeys('{F11}');
            }
        }
    }
    //Print
    static printContent(content) {
        var iframe = document.getElementById('ifmPrint');
        if (!iframe) {
            iframe = document.createElement('iframe');
            iframe.id = 'ifmPrint';
            document.body.appendChild(iframe);
        }
        var doc = iframe.contentWindow.document;
        doc.open();
        doc.write(content);
        doc.close();
        iframe.contentWindow.print();
    }
    //Download
    static async downloadFileByUrl(fileName, url) {
        const anchor = document.createElement('a');
        anchor.href = url;
        if (fileName) {
            anchor.download = fileName;
        }
        document.body.appendChild(anchor);
        anchor.click();
        anchor.remove();
    }
    static async downloadFileByStream(fileName, stream) {
        const buffer = await stream.arrayBuffer();
        const blob = new Blob([buffer], { type: 'application/octet-stream' });
        const url = URL.createObjectURL(blob);
        KBlazor.downloadFileByUrl(fileName, url);
        URL.revokeObjectURL(url);
    }
    //Pdf
    static async showPdfByUrl(id, url, option) {
        PDFObject.embed(url, '#' + id, option);
    }
    static async showPdfByStream(id, stream, option) {
        const buffer = await stream.arrayBuffer();
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        PDFObject.embed(url, '#' + id, option);
        URL.revokeObjectURL(url);
    }
    //Image
    static captcha(id, code) {
        var canvas = document.getElementById(id);
        createCaptcha(canvas, code);
    }
    //Chart
    static showBarcode(id, value, option) { JsBarcode('#' + id, value, option); }
    static showQRCode(id, option) { $('#' + id).qrcode(option); }
    //Spin
    static showSpin(tip) {
        var html = '<div class="mask"></div>';
        html += '<div class="spin">';
        html += '<span class="ant-spin-dot ant-spin-dot-spin">';
        html += '<i class="ant-spin-dot-item"></i>';
        html += '<i class="ant-spin-dot-item"></i>';
        html += '<i class="ant-spin-dot-item"></i>';
        html += '<i class="ant-spin-dot-item"></i>';
        html += '</span>';
        if (tip)
            html += '<div class="ant-spin-text">' + tip + '</div>';
        html += '</div>';
        $('<div>').attr('id', 'kuiSpin').html(html).appendTo($('body'));
    }
    static hideSpin() {
        $('#kuiSpin').remove();
    }
}