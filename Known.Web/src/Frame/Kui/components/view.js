/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

function View(name, option) {
    var _this = this,
        _refresh = option.refresh || function () { _this.load(); },
        _columns = option.columns || [],
        _form, _formOption = option.formOption || {},
        _grid, _gridOption = option.gridOption || {};

    if (_columns.filter(function (c) { return c.type; }).length) {
        _initExtForm();
        _initForm();
    }
    _initGrid();

    //property
    this.name = name;
    this.option = option;
    this.form = _form;
    this.grid = _grid;

    //method
    this.render = function () {
        var elem = $('<div>').addClass('fit');
        var view;

        if (option.left) {
            var left = $('<div>').addClass('fit-col-3').appendTo(elem);
            view = $('<div>').addClass('fit-col-7').appendTo(elem);
            option.left.render().appendTo(left);
        } else if (option.inner) {
            view = elem;
        } else {
            view = $('<div>').addClass('fit-col').appendTo(elem);
        }

        _grid.render().appendTo(view);

        if (_form) {
            _form.render().appendTo($('body'));
        }

        return elem;
    }

    this.load = function (where) {
        if (option.left) {
            option.left.reload(where);
            _grid.setData([]);
        } else {
            _grid.reload(where);
        }
    }

    this.loadGrid = function (where) {
        _grid.reload(where);
    }

    this.getGridData = function () {
        return _grid.getData();
    }

    this.setGridData = function (data) {
        _grid.setData(data);
    }

    this.setGridDetail = function (isDetail) {
        _grid.setDetail(isDetail);
    }

    this.getRowData = function (filter) {
        var rows = _grid.data.filter(filter);
        if (!rows.length)
            return null;

        return rows.length === 1 ? rows[0] : rows;
    }

    this.showForm = function (option) {
        var data = _formOption.data;
        _form.render().appendTo($('body'));
        _form.show(data, false, option);
    }

    //private
    function _initExtForm() {
        var extForm = top.ExtForm[name];
        if (extForm) {
            var extColumns = extForm.columns;
            if (extColumns && extColumns.length) {
                for (var i = 0; i < extColumns.length; i++) {
                    _columns.push(extColumns[i]);
                }
            }

            if (extForm.formOption) {
                $.extend(true, _formOption, extForm.formOption);
            }

            if (extForm.gridOption) {
                $.extend(true, _gridOption, extForm.gridOption);
            }
        }
    }

    function _initForm() {
        var formOption = $.extend(true, {
            card: true,
            fields: _columns.slice(),
            toolbar: [{
                text: Language.Save, icon: 'fa fa-save', handler: function (e) {
                    e.form.save(option.url.SaveModel, _refresh);
                }
            }]
        }, _formOption);

        if (option.formToolbar) {
            for (var i = 0; i < option.formToolbar.length; i++) {
                formOption.toolbar.push(option.formToolbar[i]);
            }
        }

        _form = new Form(name, formOption);
    }

    function _initGrid() {
        var toolButtons = [];
        if (!_gridOption.toolButtons) {
            var tab = getCurTab();
            if (tab && tab.children) {
                toolButtons = tab.children.filter(function (d) { return d.type === 'button'; });
            }
        }

        var gridOption = $.extend(true, {
            form: _form, isTradition: isTraditionView, isIconButton: false,
            url: option.url.QueryModels || '', autoQuery: false,
            toolButtons: toolButtons,
            toolbar: {
                add: function (e) {
                    var data = _form.option.data;
                    if (typeof data === 'function')
                        data = data();
                    e.addRow(data);
                },
                edit: function (e) {
                    e.editRow();
                },
                remove: function (e) {
                    e.deleteRows(option.url.DeleteModels, _refresh);
                },
                up: function (e) {
                    e.moveRow('up', option.url.MoveModel);
                },
                down: function (e) {
                    e.moveRow('down', option.url.MoveModel);
                },
                import: function (e) {
                    e.import(option.url.ImportModels);
                },
                export: function (e) {
                    e.export();
                }
            },
            columns: _columns.slice().filter(function (d) { return !d.onlyForm; })
        }, _gridOption);

        _grid = new Grid(name, gridOption);
    }
}