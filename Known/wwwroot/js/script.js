import "./libs/highcharts.js";
import "./libs/highcharts-more.js";

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

function findScript(match) {
    let items = document.getElementsByTagName('script');
    return Array.from(items).find((item) => item.getAttribute('src')?.match(match));
}

function createScript(src) {
    if (findScript(src)) return null;
    var script = document.createElement('script');
    script.src = src;
    return script;
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
    static initStaticFile(styles, scripts) {
        let known = findLink('Known');
        if (!known) {
            var app = findLink('app');
            for (var i in styles) {
                var link = createLink(styles[i]);
                if (link) {
                    document.head.insertBefore(link, app);
                }
            }
            var frame = findScript('_framework');
            for (var i in scripts) {
                var script = createScript(scripts[i]);
                if (script) {
                    document.body.insertBefore(script, frame);
                }
            }
        }
    }
    static setStyleSheet(match, href) {
        let item = findLink(match);
        if (!item) {
            item = createLink('');
            document.head.appendChild(item);
        }
        item.href = href;
    }
    static insertStyleSheet(match, href) {
        var item1 = createLink(href);
        if (item1) {
            let item = findLink(match);
            document.head.insertBefore(item1, item);
        }
    }
    static addStyleSheet(href) {
        let item = createLink(href);
        if (item) {
            document.head.appendChild(item);
        }
    }
    static removeStyleSheet(href) {
        let item = findLink(href);
        if (item) {
            document.head.removeChild(item);
        }
    }
    static setTheme(theme) {
        if (!theme)
            theme = KBlazor.getLocalStorage('Known_Theme');
        if (!theme) {
            var hour = new Date().getHours();
            theme = hour > 6 && hour < 20 ? "light" : "dark";
        }
        $('html').attr('data-theme', theme);
        KBlazor.setLocalStorage('Known_Theme', theme);
        var darkUrl = '_content/AntDesign/css/ant-design-blazor.dark.css';
        if (theme == 'dark')
            KBlazor.insertStyleSheet('/Known/css/size/', darkUrl);
        else
            KBlazor.removeStyleSheet(darkUrl);
        return theme;
    }
    static setUserSetting(setting) {
        KBlazor.setStyleSheet('/theme/', '_content/Known/css/theme/' + setting.themeColor + '.css');
        KBlazor.setStyleSheet('/size/', '_content/Known/css/size/' + setting.size + '.css');
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
    static previewImage(inputElem, imgElem) {
        const url = URL.createObjectURL(inputElem.files[0]);
        imgElem.addEventListener('load', () => URL.revokeObjectURL(url), { once: true });
        imgElem.src = url;
    }
    static previewImageById(inputElem, imgId) {
        const url = URL.createObjectURL(inputElem.files[0]);
        var imgElem = document.getElementById(imgId);
        imgElem.addEventListener('load', () => URL.revokeObjectURL(url), { once: true });
        imgElem.src = url;
    }
    static captcha(id, code) {
        var canvas = document.getElementById(id);
        createCaptcha(canvas, code);
    }
    //Chart
    static showChart(id, option) {
        //console.log(option);
        Highcharts.chart(id, option);
    }
    static showBarcode(id, value, option) { JsBarcode('#' + id, value, option); }
    static showQRCode(id, option) { $('#' + id).qrcode(option); }
    static highlight(code, language) {
        if (!code) return '';
        return Prism.highlight(code, Prism.languages[language], language);
    }
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