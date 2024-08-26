﻿import "./libs/jquery.js";
import "./libs/pdfobject.js";
import "./libs/highcharts.js";
import "./libs/barcode.js";
import "./libs/qrcode.js";
import "./libs/prism.js";

export class KBlazor {
    //Callback
    static runScript(script) {
        return eval(script);
    }
    static runScriptVoid(script) {
        eval(script);
    }
    static invokeDotNet(id, key, param) {
        return DotNet.invokeMethodAsync('Known', 'CallbackByParamAsync', id, key, param);
    }
    //Common
    static elemClick(id) {
        document.getElementById(id).click();
    }
    static elemEnabled(id, enabled) {
        document.getElementById(id).enabled = enabled;
    }
    static highlight(code, language) {
        return Prism.highlight(code, Prism.languages[language], language);
    }
    static setStyle(match, href) {
        let item = Array.from(document.getElementsByTagName('link')).find((item) =>
            item.getAttribute('href')?.match(match)
        );
        if (!item) {
            item = document.createElement('link');
            item.rel = 'stylesheet';
            document.head.appendChild(item);
        }
        item.href = href;
    }
    static setTheme(theme) {
        $('html').attr('theme', theme);
    }
    //Storage
    static getLocalStorage(key) {
        return localStorage.getItem(key);
    }
    static setLocalStorage(key, value) {
        if (value)
            localStorage.setItem(key, value);
        else
            localStorage.removeItem(key);
    }
    static getSessionStorage(key) {
        return sessionStorage.getItem(key);
    }
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
        const blob = new Blob([buffer]);
        const url = URL.createObjectURL(blob);
        KBlazor.downloadFileByUrl(fileName, url);
        URL.revokeObjectURL(url);
    }
    //Pdf
    static async showPdf(id, stream) {
        const buffer = await stream.arrayBuffer();
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        PDFObject.embed(url, '#' + id, { forceIframe: true });
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
    //Chart
    static showChart(id, option) {
        Highcharts.chart(id, option);
    }
    static showBarcode(id, value, option) {
        JsBarcode('#' + id, value, option);
    }
    static showQRCode(id, option) {
        $('#' + id).qrcode(option);
    }
}