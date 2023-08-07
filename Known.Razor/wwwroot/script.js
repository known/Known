/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * WebSite: known.pumantech.com
 * Contact: knownchen@163.com
 * ------------------------------------------------------------------------------- */

import "./libs/jquery.js";
import "./libs/flow.js";
import "./libs/barcode.js";
import "./libs/qrcode.js";
import "./libs/highcharts.js";
import "./libs/pdfobject.js";
import "./libs/xlsxcore.js";
import "./libs/wangEditor.js";
import "./js/kadmin.js";
import "./js/kform.js";
import "./js/keditor.js";

$(function () {
    $(document).click(function (e) {
        if ($(e.target).hasClass('qvtrigger') ||
            $(e.target).parents('.qvtrigger').length > 0 ||
            $(e.target).hasClass('quickview') ||
            $(e.target).parents('.quickview').length > 0)
            return;
        $('.quickview').removeClass('active');
    });
});

export class KRazor {
    //Alert
    static showNotify(message, style, timeout) {
        var tips = $('<div>').addClass('notify animated fadeInRight').addClass(style).html(message).appendTo($('body'));
        setTimeout(function () {
            tips.addClass('fadeOutRight');
            setTimeout(function () { tips.remove(); }, 500);
        }, timeout);
    }
    static showToast(message, style) {
        var tips = $('<div>').addClass('toast animated fadeInDown')
            .html('<span class="' + style + '">' + message + '</span>')
            .appendTo($('body'));
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
    static showBarcode(id, value, option) {
        JsBarcode('#' + id, value, option);
    }
    static showQRCode(id, option) {
        $('#' + id).qrcode(option);
    }

    //Dialog
    static setDialogMove(id) {
        KAdmin.setDialogMove(id);
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
    static initForm() {
        KForm.init();
    }
    static initEditor(id, option) {
        return KEditor.init(id, option);
    }
    static setFormList() {
        KForm.setFormList();
    }
    static captcha(id, code) {
        KForm.captcha(id, code);
    }

    //Grid
    static initTable(id) {
        $(window).resize(function () { KAdmin.setTable(id); });
    }
    static setTable(id) {
        KAdmin.setTable(id);
    }
    static fixedTable(id) {
        KAdmin.fixedTable(id);
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

    //Page
    static initAdminTab() {
        $('.btn-left').click(KAdmin.tabScrollLeft);
        $('.btn-right').click(KAdmin.tabScrollRight);
    }
    static initPage(id) {
        KAdmin.layout(id);
        $(window).resize(function () { KAdmin.layout(id); });
    }

    //UI
    static initMenu() {
        $('.menu-tree .item').click(function (e) {
            if ($(this).hasClass('active') && $(this).parent().hasClass('child')) {
                $(this).removeClass('active');
            } else {
                $(this).parent().parent().find('.item').removeClass('active');
                $(this).addClass('active');
            }
        });
    }
    static layout(id) {
        KAdmin.layout(id);
    }
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
    static copyToClipboard(text) {
        navigator.clipboard.writeText(text).then(function () {
            KRazor.showToast('Copied!', 'success');
        });
    }
}