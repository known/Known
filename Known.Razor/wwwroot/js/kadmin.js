window.KAdmin = {
    invokeDotNet: function (id, key, param) {
        return DotNet.invokeMethodAsync('Known.Razor', 'CallbackByParamAsync', id, key, param);
    },
    tabScrollLeft: function () {
        const dom = document.querySelector('#tabAdmin');
        dom.scrollLeft -= 120;
    },
    tabScrollRight: function () {
        const dom = document.querySelector('#tabAdmin');
        dom.scrollLeft += 120;
    },
    layout: function (id) {
        KAdmin.setTable(id);
        KAdmin.setFormList();
        KAdmin.setDialog();
    },
    setTable: function (id) {
        var gridView = $('#' + id);
        var prev = gridView.prev();
        var top = 10;
        if (prev.length && gridView.outerWidth() < 768) {
            top = prev.outerHeight();
        }
        gridView.css('top', top + 'px');

        top = 0;
        var toolbar = $('#' + id + ' .data-top');
        var grid = $('#' + id + ' .grid');
        if (toolbar.length && grid.length) {
            top = toolbar.outerHeight() + 8;
        }
        grid.css('top', top + 'px');

        var table = $('#' + id + ' table');
        if (table.length && grid.length) {
            var tableWidth = table.outerWidth();
            var gridWidth = grid.outerWidth();
            if (tableWidth < gridWidth)
                table.css({ width: '100%' });
            else
                table.css({ width: tableWidth + 'px' });
        }
    },
    fixedTable: function (id) {
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
    },
    div: {},
    setDialogMove: function (id) {
        var layer = $('#' + id);
        $('#' + id + ' .dlg-head').mousedown(function (e) {
            e.preventDefault();
            if (layer.hasClass('max'))
                return;

            KAdmin.div.id = id;
            KAdmin.div.move = true;
            KAdmin.div.offset = [
                e.clientX - parseFloat(layer.css('left')),
                e.clientY - parseFloat(layer.css('top'))
            ];
        }).mousemove(function (e) {
            e.preventDefault();
            if (KAdmin.div.id === id && KAdmin.div.move) {
                var left = e.clientX - KAdmin.div.offset[0];
                var top = e.clientY - KAdmin.div.offset[1];
                layer.css({ left: left, top: top });
            }
        }).mouseup(function () {
            delete KAdmin.div.move;
        });
    }
};