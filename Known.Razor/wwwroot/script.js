/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-04-01     KnownChen
 * ------------------------------------------------------------------------------- */

// This is a JavaScript module that is loaded on demand. It can export any number of
// functions, and may import other JavaScript modules if required.
import "./libs/jquery.js";
import "./libs/flow.js";
import "./libs/highcharts.js";
import "./libs/pdfobject.js";
import "./libs/wangEditor.js";
import "./libs/xlsxcore.js";

$(function () {
    $(document).click(function (e) {
        if ($(e.target).hasClass('quickview') ||
            $(e.target).hasClass('qvtrigger') ||
            $(e.target).parents('.quickview').length > 0)
            return;
        $('.quickview').removeClass('active');
    });
});

export class KRazor {
    //Alert
    static showTips(message) {
        var tips = $('<div>').addClass('dlg-tips animated fadeInDown').html(message).appendTo($('body'));
        setTimeout(function () {
            tips.addClass('fadeOutUp');
            setTimeout(function () { tips.remove(); }, 500);
        }, 3000);
    }

    //Flow
    static showFlow(info) {
        Flowcharts.chart(info);
    }

    //Chart
    static showChart(info) {
        Highcharts.chart(info.id, info.option);
    }

    //Dialog
    static div = {};
    static setDialogMove(dialogId) {
        var layer = $('#' + dialogId);
        $('#' + dialogId + ' .dlg-head').mousedown(function (e) {
            e.preventDefault();
            if (layer.hasClass('max'))
                return;

            KRazor.div.id = dialogId;
            KRazor.div.move = true;
            KRazor.div.offset = [
                e.clientX - parseFloat(layer.css('left')),
                e.clientY - parseFloat(layer.css('top'))
            ];
        }).mousemove(function (e) {
            e.preventDefault();
            if (KRazor.div.id === dialogId && KRazor.div.move) {
                var left = e.clientX - KRazor.div.offset[0];
                var top = e.clientY - KRazor.div.offset[1];
                layer.css({ left: left, top: top });
            }
        }).mouseup(function () {
            delete KRazor.div.move;
        });
    }

    //Excel
    static async excelImport(stream) {
        var data = await stream.arrayBuffer();
        var book = XLSX.read(data, { type: 'binary' });
        var sheetNames = book.SheetNames;
        var sheet = book.Sheets[sheetNames[0]];
        return XLSX.utils.sheet_to_txt(sheet);
    }
    static excelExport(fileName, datas) {
        //console.log(XLSX);
        var sheet = XLSX.utils.aoa_to_sheet(datas);
        var book = XLSX.utils.book_new();
        XLSX.utils.book_append_sheet(book, sheet, 'Sheet1');
        XLSX.writeFile(book, fileName);
    }

    //File
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
        KRazor.downloadFileByUrl(fileName, url);
        URL.revokeObjectURL(url);
    }
    static async showImage(id, stream) {
        const buffer = await stream.arrayBuffer();
        const blob = new Blob([buffer]);
        const url = URL.createObjectURL(blob);
        $('#' + id).attr('src', url);
    }
    static async showPdf(id, stream) {
        const buffer = await stream.arrayBuffer();
        const blob = new Blob([buffer], { type: 'application/pdf' });
        const url = URL.createObjectURL(blob);
        PDFObject.embed(url, '#' + id, { forceIframe: true });
        URL.revokeObjectURL(url);
    }

    //Form
    static bindEnter() {
        var inputs = $('.form input');
        inputs.keydown(function (event) {
            if ((event.keyCode || event.which) === 13) {
                event.preventDefault();
                var index = inputs.index(this);
                if (index < inputs.length)
                    $(inputs[index + 1]).focus();
                var method = $(this).attr("onenter");
                if (method && method.length)
                    eval(method);
            }
        });
    }

    //Grid
    static fixedTable(id) {
        var table = $('#' + id);
        var left = 0;
        var fixeds = table.find('th.fixed');
        if (fixeds.length) {
            var lefts = [];
            for (var i = 0; i < fixeds.length; i++) {
                lefts.push(left);
                left += fixeds[i].clientWidth;
            }
            var trs = table.find('tr');
            if (trs.length) {
                for (var i = 0; i < trs.length; i++) {
                    var tr = trs[i];
                    for (var j = 0; j < lefts.length; j++) {
                        $(tr).find('.fixed:eq(' + j + ')').css({ left: lefts[j] });
                    }
                }
            }
        }
    }

    //Storage
    static getLocalStorage(key) {
        return localStorage.getItem(key);
    }
    static setLocalStorage(key, value) {
        if (value)
            localStorage.setItem(key, JSON.stringify(value));
        else
            localStorage.removeItem(key);
    }
    static getSessionStorage(key) {
        return sessionStorage.getItem(key);
    }
    static setSessionStorage(key, value) {
        if (value)
            sessionStorage.setItem(key, JSON.stringify(value));
        else
            sessionStorage.removeItem(key);
    }

    //UI
    static appendBody(html) {
        $('body').append(html);
    }
    static showFrame(id, url) {
        $('#' + id).attr('src', url);
    }
    static showLoading() {
        document.body.classList.add('loading');
    }
    static hideLoading() {
        document.body.classList.remove('loading');
    }
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
    static openLink(url) {
        window.open(url, '_blank');
    }
    static elemClick(id) {
        document.getElementById(id).click();
    }
    static elemEnabled(id, enabled) {
        $('#' + id).attr('disabled', !enabled);
    }
    static toggleClass(id, className) {
        var elem = $('#' + id);
        if (elem.hasClass(className))
            elem.removeClass(className);
        else
            elem.addClass(className);
    }
}