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

export function hello(name) {
    DotNet.invokeMethodAsync('Known', 'Hello', name).then(data => {
        alert(data);
    });
}

//Alert
export function KR_showTips(message) {
    var tips = $('<div>').addClass('dlg-tips animated fadeInDown').html(message).appendTo($('body'));
    setTimeout(function () { tips.remove() }, 3000);
}

//Flow
export function KR_showFlow(info) {
    Flowcharts.chart(info);
}

//Chart
export function KR_showChart(info) {
    //console.log(info);
    Highcharts.chart(info.id, info.option);
}

//Dialog
var KR_div = {};
export function KR_setDialogMove(dialogId) {
    var layer = $('#' + dialogId);
    $('#' + dialogId + ' .dlg-head').mousedown(function (e) {
        e.preventDefault();
        if (layer.hasClass('max'))
            return;

        KR_div.id = dialogId;
        KR_div.move = true;
        KR_div.offset = [
            e.clientX - parseFloat(layer.css('left')),
            e.clientY - parseFloat(layer.css('top'))
        ];
    }).mousemove(function (e) {
        e.preventDefault();
        if (KR_div.id === dialogId && KR_div.move) {
            var left = e.clientX - KR_div.offset[0];
            var top = e.clientY - KR_div.offset[1];
            layer.css({ left: left, top: top });
        }
    }).mouseup(function () {
        delete KR_div.move;
    });
}

//Excel
export async function KR_excelImport(stream) {
    var data = await stream.arrayBuffer();
    var book = XLSX.read(data, { type: 'binary' });
    var sheetNames = book.SheetNames;
    var sheet = book.Sheets[sheetNames[0]];
    return XLSX.utils.sheet_to_txt(sheet);
}

export function KR_excelExport(fileName, datas) {
    //console.log(XLSX);
    var sheet = XLSX.utils.aoa_to_sheet(datas);
    var book = XLSX.utils.book_new();
    XLSX.utils.book_append_sheet(book, sheet, 'Sheet1');
    XLSX.writeFile(book, fileName);
}

//File
export function KR_printContent(content) {
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

export async function KR_downloadFileByUrl(fileName, url) {
    const anchor = document.createElement('a');
    anchor.href = url;
    if (fileName) {
        anchor.download = fileName;
    }
    document.body.appendChild(anchor);
    anchor.click();
    anchor.remove();
}

export async function KR_downloadFileByStream(fileName, stream) {
    const buffer = await stream.arrayBuffer();
    const blob = new Blob([buffer]);
    const url = URL.createObjectURL(blob);
    KR_downloadFileByUrl(fileName, url);
    URL.revokeObjectURL(url);
}

export async function KR_showImage(id, stream) {
    const buffer = await stream.arrayBuffer();
    const blob = new Blob([buffer]);
    const url = URL.createObjectURL(blob);
    $('#' + id).attr('src', url);
}

export async function KR_showPdf(id, stream) {
    const buffer = await stream.arrayBuffer();
    const blob = new Blob([buffer], { type: 'application/pdf' });
    const url = URL.createObjectURL(blob);
    PDFObject.embed(url, '#' + id, { forceIframe: true });
    URL.revokeObjectURL(url);
}

//Form
export function KR_bindEnter() {
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
export function KR_toggleQuery(id) {
    var btn = $('#' + id);
    var query = btn.parent();
    var tool = query.parent().find('.tool');
    if (btn.text() === '更多') {
        btn.text('收起');
        query.addClass('more');
    } else {
        btn.text('更多');
        query.removeClass('more');
    }
    var queryHeight = query.outerHeight();
    var toolHeight = tool.length ? tool.outerHeight() : 0;
    query.parent().find('.grid').css({ top: (queryHeight + toolHeight) + 'px' });
}

export function KR_fixedTable(id) {
    var table = $('#' + id);
    var left = 0;
    var index = table.find('th.index');
    if (index.length) {
        table.find('.index').css({ left: left });
        left += index[0].clientWidth;
    }
    var check = table.find('th.check');
    if (check.length) {
        table.find('.check').css({ left: left });
        left += check[0].clientWidth;
    }
    var action = table.find('th.action');
    if (action.length) {
        table.find('.action').css({ left: left });
        left += action[0].clientWidth;
    }
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
export function KR_getLocalStorage(key) {
    return localStorage.getItem(key);
}

export function KR_setLocalStorage(key, value) {
    if (value)
        localStorage.setItem(key, JSON.stringify(value));
    else
        localStorage.removeItem(key);
}

export function KR_getSessionStorage(key) {
    return sessionStorage.getItem(key);
}

export function KR_setSessionStorage(key, value) {
    if (value)
        sessionStorage.setItem(key, JSON.stringify(value));
    else
        sessionStorage.removeItem(key);
}

//UI
export function KR_appendBody(html) {
    $('body').append(html);
}

export function KR_showFrame(id, url) {
    $('#' + id).attr('src', url);
}

export function KR_showLoading() {
    document.body.classList.add('loading');
}

export function KR_hideLoading() {
    document.body.classList.remove('loading');
}

export function KR_openFullScreen() {
    var el = document.documentElement;
    var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
    if (rfs) {
        rfs.call(el);
    } else if (typeof window.ActiveXObject !== 'undefined') {
        //for IE
        var wscript = new ActiveXObject('WScript.Shell');
        if (wscript !== null) {
            wscript.SendKeys('{F11}');
        }
    }
}

export function KR_closeFullScreen() {
    var el = document;
    var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
    if (cfs) {
        cfs.call(el);
    } else if (typeof window.ActiveXObject !== 'undefined') {
        //for IE
        var wscript = new ActiveXObject('WScript.Shell');
        if (wscript !== null) {
            wscript.SendKeys('{F11}');
        }
    }
}

export function KR_openLink(url) {
    window.open(url, '_blank');
}

export function KR_elemClick(id) {
    document.getElementById(id).click();
}

export function KR_elemEnabled(id, enabled) {
    $('#' + id).attr('disabled', !enabled);
}

export function KR_toggleClass(id, className) {
    var elem = $('#' + id);
    if (elem.hasClass(className))
        elem.removeClass(className);
    else
        elem.addClass(className);
}