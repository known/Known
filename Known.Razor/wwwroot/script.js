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

window.KAdmin = {
    scrollLeft: function () {
        const dom = document.querySelector('#tabAdmin');
        dom.scrollLeft -= 120;
    },
    scrollRight: function () {
        const dom = document.querySelector('#tabAdmin');
        dom.scrollLeft += 120;
    },
    layout: function (id) {
        KAdmin.setTable(id);
        KAdmin.setFormList();
        KAdmin.setDialog();
    },
    setTable: function (id) {
        var toolbar = $('#' + id + ' .data-top');
        var grid = $('#' + id + ' .grid');
        var table = $('#' + id + ' table');
        if (toolbar.length && grid.length) {
            var top = toolbar.outerHeight() + 8;
            grid.css('top', top + 'px');
        }
        if (table.length && grid.length) {
            var tableWidth = table.outerWidth();
            var gridWidth = grid.outerWidth();
            if (tableWidth < gridWidth)
                table.css({ width: '100%' });
            else
                table.css({ width: tableWidth + 'px' });
        }
    },
    setFormList: function () {
        var list = $('.form-list');
        if (!list.length)
            return;

        var prev = list.prev();
        var top = prev.position().top + prev.outerHeight(true);
        list.css('top', top + 'px');
    },
    setDialog: function () {
        var dialog = $('.dialog:not(.max)');
        if (!dialog.length)
            return;

        var width = document.body.clientWidth;
        dialog.each(function (i, elem) {
            var dlg = $(elem);
            if (width < 786) {
                if (!dlg.hasClass('full')) {
                    dlg.addClass('full');
                    dlg.data('style', dlg.attr('style'));
                    var zIndex = dlg.css('z-index');
                    var topColor = dlg.css('border-top-color');
                    dlg.attr('style', 'z-index:' + zIndex + ';border-top-color:' + topColor);
                }
            } else {
                if (dlg.hasClass('full')) {
                    dlg.removeClass('full');
                    dlg.attr('style', dlg.data('style'));
                }
            }
        });
    }
};

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
    static initForm() {
        var inputs = $('.form input');
        if (inputs.length) {
            inputs.keydown(function (event) {
                if ((event.keyCode || event.which) === 13) {
                    event.preventDefault();
                    var index = inputs.index(this);
                    if (index < inputs.length - 1)
                        inputs[index + 1].focus();
                    this.blur();
                    var method = $(this).attr("onenter");
                    if (method && method.length)
                        eval(method);
                }
            });
            inputs[0].focus();
        }
        KAdmin.setFormList();
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
        ctx.font = width / 5 + 'px Î¢ÈíÑÅºÚ';
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
            var max = a;
            var min = b;
            if (a < b) {
                max = b;
                min = a;
            }
            return Math.floor(Math.random() * (max - min)) + min;
        }

        function getColor() {
            return `rgb(${Math.floor(Math.random() * 255)},${Math.floor(Math.random() * 256)},${Math.floor(Math.random() * 256)})`;
        }
    }

    //Grid
    static initTable(id) {
        $(window).resize(function () { KAdmin.setTable(id); });
    }
    static setTable(id) {
        KAdmin.setTable(id);
    }
    static fixedTable(id) {
        var table = $('#' + id);
        var left = 0;
        var fixeds = table.find('th.fixed');
        if (fixeds.length) {
            var lefts = [];
            for (var i = 0; i < fixeds.length; i++) {
                lefts.push(left);
                left += $(fixeds[i]).outerWidth();
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

    //Page
    static initAdminTab() {
        $('.btn-left').click(KAdmin.scrollLeft);
        $('.btn-right').click(KAdmin.scrollRight);
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