GridManager.isLoadXlsx = false;
function GridManager(grid, form, rows) {
    //field
    var title = form ? form.title : '';
    if (!title) {
        var tab = getCurTab();
        title = tab ? tab.title : '';
    }

    //property
    this.grid = grid;
    this.rows = rows;
    this.row = rows && rows.length ? rows[0] : null;

    //public
    this.selectRow = function (callback) {
        if (!rows || rows.length === 0 || rows.length > 1) {
            Layer.tips(Language.PleaseSelectOne);
            return;
        }

        var row = rows[0];
        callback && callback({
            grid: grid, form: form,
            rows: rows, row: row,
            index: row.index, ids: [row.Id]
        });
    }

    this.selectRows = function (callback) {
        if (!rows || rows.length === 0) {
            Layer.tips(Language.PleaseSelectOneAtLeast);
            return;
        }

        var ids = rows.map(function (d) { return d.Id; });
        callback && callback({
            grid: grid, form: form,
            rows: rows, ids: ids
        });
    }

    this.addRow = function (data) {
        form.show(data);
    }

    this.editRow = function (callback) {
        this.selectRow(function (e) {
            if (callback && !callback(e))
                return;

            form.show(e.row);
            if (grid.option.formUrl) {
                form.load(grid.option.formUrl + '?id=' + e.row.Id);
            }
        });
    }

    this.deleteRow = function (url, callback) {
        this.selectRow(function (e) {
            deleteData(e, url, callback);
        });
    }

    this.deleteRows = function (url, callback) {
        this.selectRows(function (e) {
            deleteData(e, url, callback);
        });
    }

    this.operateRow = function (url, name, callback) {
        this.selectRow(function (e) {
            var data = { id: e.row.Id };
            var text = name ? Utils.format(Language.ConfirmRecord, name) : '';
            if (text) {
                Layer.confirm(text, function () {
                    operateData(e, url, data, callback);
                });
            } else {
                operateData(e, url, data, callback);
            }
        });
    }

    this.operateRows = function (url, name, callback) {
        this.selectRows(function (e) {
            var data = { data: JSON.stringify(e.ids) };
            var text = name ? Utils.format(Language.ConfirmRecord, name) : '';
            if (text) {
                Layer.confirm(text, function () {
                    operateData(e, url, data, callback);
                });
            } else {
                operateData(e, url, data, callback);
            }
        });
    }

    this.moveRow = function (direct, url) {
        this.selectRow(function (e) {
            e.grid.moveRow(e.index, direct, url);
        });
    }

    this.import = function (url, param) {
        loadXlsxCore();
        var gridImport;
        var columns = grid.columns.slice().filter(function (d) { return d.import; });
        Layer.open({
            title: title + '【' + Language.Import + '】', width: 1024, height: 450,
            content: function (dom) {
                var div = $('<div>').addClass('import-tips').appendTo(dom);
                div.append(Language.ImportTips);
                $('<div>')
                    .addClass('import-body')
                    .append('<div class="grid"><table id="gridImport"></table></div>')
                    .appendTo(dom);
            },
            success: function () {
                var importColumns = [{
                    title: Language.ValidInfo, field: 'Error', format: function (d, e) {
                        if (d.Error && d.Error.length) {
                            e.tr.css({ color: '#f00' });
                            return '<span class="red">' + d.Error + '</span>';
                        }
                        return '';
                    }
                }].concat(columns);
                gridImport = new Grid('Import', { import: true, columns: importColumns });
                Utils.paste(true, function (text) {
                    var rows = [], lines = text.split('\n');
                    if (lines.length > 1000) {
                        Layer.tips(Language.ImportMaxLength);
                        return;
                    }

                    for (var i = 0; i < lines.length; i++) {
                        if (!lines[i].length)
                            continue;

                        var row = $.extend({}, form.data), line = lines[i].replace('\r', '').split('\t');
                        for (var j = 0; j < columns.length; j++) {
                            if (line.length > j) {
                                row[columns[j].field] = line[j];
                            }
                        }
                        rows.push(row);
                    }
                    gridImport.setData(rows);
                });
            },
            buttons: [{
                text: Language.ImportRule, handler: function (dlg) {
                    var aoa = [], col = [];
                    for (var i = 0; i < columns.length; i++) {
                        col.push(columns[i].title);
                    }
                    aoa.push(col);
                    var sheet = XLSX.utils.aoa_to_sheet(aoa);
                    Utils.genFile(sheet2Text(sheet), title + Language.ImportRule + '.xlsx');
                }
            }, {
                    text: Language.Import, handler: function (dlg) {
                    var data = gridImport.getData();
                    if (!data.length) {
                        Layer.tips(Language.PleasePasteImport);
                        return;
                    }

                    if (!gridImport.validate()) {
                        Layer.tips(Language.DataNotValidPleaseCheck);
                        return;
                    }

                    var loading = Layer.loading(Language.Importing + '......');
                    var postData = $.extend({}, param, { data: JSON.stringify(data) });
                    $.post(url, postData, function (result) {
                        loading.close();
                        if (result.IsValid) {
                            Layer.tips(result.Message);
                            dlg.close();
                            Utils.paste(false);
                            grid.reload();
                        } else {
                            Layer.alert(result.Message);
                            var errors = result.data;
                            if (errors && errors.length) {
                                var datas = gridImport.data;
                                for (var i = 0; i < datas.length; i++) {
                                    datas[i].Error = errors[i];
                                }
                                gridImport.setData(datas);
                            }
                        }
                    });
                }
            }]
        });
    }

    this.export = function (fileName) {
        loadXlsxCore();
        //var aoa = [
        //    ['姓名', '性别', '年龄', '注册时间'],
        //    ['张三', '男', 18, new Date()],
        //    ['李四', '女', 22, new Date()]
        //];

        var aoa = [], col = [], columns = grid.columns;
        for (var i = 0; i < columns.length; i++) {
            if (columns[i].export) {
                col.push(columns[i].title);
            }
        }

        if (col.length === 0) {
            Layer.tips(Language.NotExportColumn);
            return;
        }

        var data = grid.getData();
        if (data.length === 0) {
            Layer.tips(Language.NotExportData);
            return;
        }

        aoa.push(col);

        for (var i = 0; i < data.length; i++) {
            var row = [];
            for (var j = 0; j < columns.length; j++) {
                if (columns[j].export) {
                    var value = grid.getValue(data[i], columns[j]);
                    row.push(value);
                }
            }
            aoa.push(row);
        }

        var sheet = XLSX.utils.aoa_to_sheet(aoa);
        Utils.genFile(sheet2Text(sheet), (fileName || title) + '.xlsx');
    }

    //private
    function loadXlsxCore() {
        if (!GridManager.isLoadXlsx) {
            Utils.addJs('/libs/xlsx.core.min.js');
            GridManager.isLoadXlsx = true;
        }
    }

    function deleteData(e, url, callback) {
        var length = e.rows.length;
        var msg = length > 1 ? Utils.format(Language.SelectRecord, length) : Language.TheRecord;
        var text = Utils.format(Language.ConfirmDeleteTips, msg);
        Layer.confirm(text, function () {
            operateData(e, url, { data: JSON.stringify(e.ids) }, callback);
        });
    }

    function operateData(e, url, data, callback) {
        Ajax.post(url, data, function () {
            if (callback) {
                callback(e);
            } else {
                e.grid.reload();
            }
        });
    }

    function sheet2Text(sheet, sheetName) {
        sheetName = sheetName || 'Sheet1';
        var workbook = {
            SheetNames: [sheetName],
            Sheets: {}
        };
        workbook.Sheets[sheetName] = sheet;
        var wopts = {
            bookType: 'xlsx',
            bookSST: false, // Shared String Table
            type: 'binary'
        };
        return XLSX.write(workbook, wopts);
    }
}