//============================== mini ui ====================================//
mini.Pager.prototype.sizeList = [10, 20, 50, 100, 200, 500, 1000, 1500, 2000];
mini.DataTable.prototype.pageSize = 20;
mini.DataGrid.prototype.allowAlternating = true;
mini.DataGrid.prototype.showColumnsMenu = true;
mini.DataGrid.prototype.showEmptyText = true;
//mini.DataGrid.prototype.showVGridLines = false;
mini.DataGrid.prototype.emptyText = '未查到任何数据！';

//---------------------------vtypes------------------------------------------//
mini.VTypes["plusErrorText"] = "必须大于0";
mini.VTypes["plus"] = function (v) {
    if (v !== null && v !== "")
        return v > 0;

    return true;
};

mini.VTypes["non-negativeErrorText"] = "必须大于等于0";
mini.VTypes["non-negative"] = function (v) {
    if (v !== null && v !== "")
        return v >= 0;

    return true;
};

mini.VTypes["non-zeroErrorText"] = "不能为0";
mini.VTypes["non-zero"] = function (v) {
    if (v !== null && v !== "")
        return v !== 0;

    return true;
};

mini.VTypes["percentErrorText"] = "必须大于等于0，并且要小于100";
mini.VTypes["percent"] = function (v) {
    if (v !== null && v !== "")
        return v >= 0 && v < 100;

    return true;
};

//---------------------------decimalBox------------------------------------------//
mini.DecimalBox = function () {
    mini.DecimalBox.superclass.constructor.call(this);
    this.bindEvents();
};
mini.extend(mini.DecimalBox, mini.TextBox, {

    uiCls: 'mini-decimalbox',

    bindEvents: function () {
        var that = this;

        $(this.getEl()).bind('input propertychange', function () {
            var input = that.getInputText();
            that.setValue(input);

            input = input.replace(/[^\d.]*/g, '');
            if (input.indexOf('.') === 0)
                input = '';

            var array = input.split('.');
            if (array.length - 1 > 1)
                input = array[0] + '.' + array[1];

            if (input.indexOf('0') === 0 && input.indexOf('.') !== 1)
                input = '0';

            that.setValue(input);
        });

        var oldValue;

        this.on('focus', function () {
            oldValue = that.getValue();
        });

        this.on('blur', function () {
            var value = this.getValue();

            if (value !== '') {
                var len = value.length;

                if (value.substring(len - 1) === '.')
                    this.setValue(value.substring(0, len - 1));
            }

            if (oldValue !== that.getValue()) {
                oldValue = that.getValue();
                that.doValueChanged();
            }
        });
    }

});
mini.regClass(mini.DecimalBox, 'decimalbox');

//---------------------------integerBox------------------------------------------//
mini.IntegerBox = function () {
    mini.IntegerBox.superclass.constructor.call(this);
    this.bindEvents();
};
mini.extend(mini.IntegerBox, mini.TextBox, {

    uiCls: 'mini-integerbox',

    bindEvents: function () {
        var that = this;

        $(this.getEl()).bind('input propertychange', function () {
            var input = that.getInputText();
            that.setValue(input);
            that.setValue(input.replace(/\D/g, ''));
        });

        var oldValue;

        this.on('focus', function () {
            oldValue = that.getValue();
        });

        this.on('blur', function () {
            if (oldValue !== that.getValue()) {
                oldValue = that.getValue();
                that.doValueChanged();
            }
        });
    }

});
mini.regClass(mini.IntegerBox, 'integerbox');

//---------------------------pagerTree------------------------------------------//
mini.PagerTree = function () {
    mini.PagerTree.superclass.constructor.call(this);
    mini.addClass(this.el, 'mini-tree');
    mini.addClass(this.el, 'mini-treegrid');

    this._Expander = new mini._PagerTree_Expander(this);

    this._collapseNodes = [];
    this.on("beforeload", this.__OnBeforeLoad, this);
};
mini.extend(mini.PagerTree, mini.DataGrid, {

    uiCls: 'mini-pagertree',
    treeColumn: '',
    idField: 'id',
    showTreeIcon: true,
    iconField: "iconCls",
    imgField: 'img',
    imgPath: '',

    leafIconCls: "mini-tree-leaf",
    folderIconCls: "mini-tree-folder",
    _expandNodeCls: "mini-tree-expand",
    _collapseNodeCls: "mini-tree-collapse",
    _eciconCls: "mini-tree-node-ecicon",

    _OnCellMouseDown: function (e) {
        if (mini.findParent(e.htmlEvent.target, this._eciconCls)) {
            //
        } else if (mini.findParent(e.htmlEvent.target, 'mini-tree-checkbox')) {
            //
        } else {
            this.fire("cellmousedown", e);
        }
    },

    _OnCellClick: function (e) {
        if (mini.findParent(e.htmlEvent.target, this._eciconCls)) return;

        if (mini.findParent(e.htmlEvent.target, 'mini-tree-checkbox')) {
            //
        } else {
            this.fire("cellclick", e);
        }
    },

    _OnDrawCell: function (record, column, rowIndex, columnIndex) {
        var e = mini.PagerTree.superclass._OnDrawCell.call(this, record, column, rowIndex, columnIndex);
        if (this.treeColumn && this.treeColumn == column.name) {
            e.headerCls += ' mini-tree-treecolumn';
            e.cellCls += ' mini-tree-treecell';
            e.cellStyle += ';padding:0;vertical-align:top;';

            e.img = record[this.imgField];
            e.iconCls = this._getNodeIcon(record);
            e.showTreeIcon = this.showTreeIcon;
            e.nodeCls = "";
            e.nodeStyle = "";

            this._createTreeCell(e);
        }
        return e;
    },

    _createTreeCell: function (e) {
        var sb = [];
        var node = e.record;

        var isLeaf = this.isLeafNode(node);
        var level = this.getNodeLevel(node);
        var isExpand = this.isExpandedNode(node);

        var cls = e.nodeCls;

        if (!isLeaf) {
            cls = isExpand ? this._expandNodeCls : this._collapseNodeCls;
        }

        if (!isLeaf) {
            cls += " mini-tree-parentNode";
        }

        sb[sb.length] = '<div class="mini-tree-nodetitle ' + cls + '" style="' + e.nodeStyle + '">';

        //_level
        var ii = 0;
        for (var i = ii; i < level - 1; i++) {
            sb[sb.length] = '<span class="mini-tree-indent " ></span>';
        }

        if (!isLeaf) {
            sb[sb.length] = '<a class="' + this._eciconCls + '"  href="javascript:void(0);" onclick="return false;" hidefocus></a>';
        } else {
            sb[sb.length] = '<span class="' + this._eciconCls + '" ></span>';
        }


        sb[sb.length] = '<span class="mini-tree-nodeshow">';
        if (e.showTreeIcon) {
            if (e.img) {
                var img = this.imgPath + e.img;
                sb[sb.length] = '<span class="mini-tree-icon mini-icon" style="background-image:url(' + img + ');"></span>';
            } else {
                sb[sb.length] = '<span class="' + e.iconCls + ' mini-tree-icon mini-icon"></span>';
            }
        }

        sb[sb.length] = '<span class="mini-tree-nodetext">';
        sb[sb.length] = e.cellHtml;
        sb[sb.length] = '</span>';

        sb[sb.length] = '</span>';

        sb[sb.length] = '</div>';

        e.cellHtml = sb.join('');
    },

    /////////////////////////////////////////////////////////////////////////
    __OnBeforeLoad: function (e) {
        var config = { collapseNodes: this._collapseNodes };
        e.data.__ecconfig = mini.encode(config);
    },

    load: function () {
        this._collapseNodes = [];
        return mini.PagerTree.superclass.load.apply(this, arguments);
    },

    expandNode: function (nodeId) {
        if (typeof nodeId === "object") nodeId = nodeId[this.idField];
        this._collapseNodes.remove(nodeId);
        this.reload();
    },

    collapseNode: function (nodeId) {
        if (typeof nodeId === "object") nodeId = nodeId[this.idField];
        this._collapseNodes.remove(nodeId);
        this._collapseNodes.add(nodeId);
        this.reload();
    },

    toggleNode: function (nodeId) {
        if (this.isExpandedNode(nodeId)) {
            this.collapseNode(nodeId);
        } else {
            this.expandNode(nodeId);
        }
    },

    collapseNodes: function (nodeIds) {
        for (var i = 0, l = nodeIds.length; i < l; i++) {
            var nodeId = nodeIds[i];
            this._collapseNodes.remove(nodeId);
            this._collapseNodes.add(nodeId);
        }
        this.reload();
    },

    expandNodes: function (nodeIds) {
        for (var i = 0, l = nodeIds.length; i < l; i++) {
            var nodeId = nodeIds[i];
            this._collapseNodes.remove(nodeId);
        }
        this.reload();
    },

    collapseAll: function () {
        var all = this.getResultObject().allIds || [];   ///........
        this._collapseNodes.length = 0;
        this._collapseNodes.addRange(all);
        this.reload();
    },

    expandAll: function () {
        this._collapseNodes.length = 0;
        this.reload();
    },

    /////////////////////////////////////////////////////////////////////////
    isExpandedNode: function (nodeId) {
        //return node.expanded !== false;
        if (typeof nodeId === "object") nodeId = nodeId[this.idField];
        return this._collapseNodes.indexOf(nodeId) === -1;
    },

    isLeafNode: function (node) {
        return node.isLeaf;
    },

    getNodeLevel: function (node) {
        return node._level;
    },

    _getNodeIcon: function (node) {
        var icon = node[this.iconField];
        if (!icon) {
            if (this.isLeafNode(node)) icon = this.leafIconCls;
            else icon = this.folderIconCls;
        }
        return icon;
    },

    getAttrs: function (el) {
        var attrs = mini.PagerTree.superclass.getAttrs.call(this, el);

        mini._ParseString(el, attrs,
            ["treeColumn", "iconField", "imgField", "imgPath", "idField"]
        );

        mini._ParseBool(el, attrs,
            ["showTreeIcon", "expandOnLoad"]
        );

        return attrs;
    }

});
mini.regClass(mini.PagerTree, "pagertree");

//分页树折叠插件
mini._PagerTree_Expander = function (grid) {
    this.owner = grid;
    mini.on(grid.el, "click", this.__OnClick, this);
};
mini._PagerTree_Expander.prototype = {

    __OnClick: function (e) {
        var tree = this.owner;
        var node = tree.getRecordByEvent(e, false);
        if (!node) return;
        var isLeaf = tree.isLeafNode(node);
        if (mini.findParent(e.target, tree._eciconCls)) {
            if (tree.isLeafNode(node)) return;
            tree.toggleNode(node[tree.idField]);
        }
    }

};

//---------------------------columnsMenu------------------------------------------//
var ColumnsMenu = function (grid, options) {

    var me = this;
    this.grid = grid;
    this.menu = this.createMenu();
    this.currentColumn = null;

    this.menu.on("beforeopen", this.onBeforeOpen, this);
    //grid.setHeaderContextMenu(this.menu);

    grid.on("update", function (e) {
        me._renderColumnTriggers();
    });

    $(grid.el).on("click", ".mini-grid-column-trigger", function (e) {
        e.stopPropagation();

        var column = grid.getColumnByEvent(e);
        me.showMenu(column);
    });

    me.menu.on("close", function (e) {
        $(grid.el).find(".mini-grid-column-open").removeClass("mini-grid-column-open");
    });

};
ColumnsMenu.prototype = {

    _renderColumnTriggers: function () {
        var me = this,
            grid = me.grid,
            options = me.options,
            columns = grid.getBottomColumns();

        for (var i = 0, l = columns.length; i < l; i++) {
            var column = columns[i];
            var el = grid.getHeaderCellEl(column);
            if (!el) continue;
            if (!column.field) continue;
            $(el.firstChild).append('<div class="mini-grid-column-trigger mini-icon mini-widget-header fa-sort-down" style="line-height:20px;"></div>');
        }
    },

    showMenu: function (column) {
        var me = this,
            menu = me.menu,
            grid = me.grid;

        var columnEl = grid.getHeaderCellEl(column);
        $(columnEl).addClass("mini-grid-column-open");
        var el = $(columnEl).find(".mini-grid-column-trigger")[0];

        menu.showAtEl(el, {
            xAlign: "left",
            yAlign: "below"
        });

        this.currentColumn = column;
    },

    createMenu: function () {
        var grid = this.grid;

        //创建菜单对象
        var menu = mini.create({ type: "menu", hideOnClick: false });

        var items = [
            { text: "正序", name: "asc", iconCls: "fa-sort-alpha-asc" },
            { text: "倒序", name: "desc", iconCls: "fa-sort-alpha-desc" },
            '-'
        ];

        //创建隐藏菜单列
        var columns = grid.getBottomColumns();
        var columnMenuItems = { text: "隐藏列", name: "showcolumn", iconCls: "fa-columns" };
        columnMenuItems.children = [];
        for (var i = 0, l = columns.length; i < l; i++) {
            var column = columns[i];
            if (column.hideable) continue;
            var item = {};
            item.checked = column.visible;
            item.checkOnClick = true;
            item.text = column.header;
            if (item.text === "&nbsp;") {
                if (column.type === "indexcolumn") item.text = "序号";
                if (column.type === "checkcolumn") item.text = "选择";
            }
            item.enabled = column.enabled;
            item._column = column;
            columnMenuItems.children.push(item);
        }
        items.push(columnMenuItems);

        //items.push('-');
        //items.push({ text: "过滤", name: "filter" });
        //items.push({ text: "取消过滤", name: "clearfilter" });

        menu.setItems(items);
        menu.on("itemclick", this.onMenuItemClick, this);

        $(menu.el).addClass("mini-menu-open");

        return menu;
    },

    onBeforeOpen: function (e) {
        //var grid = this.grid;
        //var column = grid.getColumnByEvent(e.htmlEvent);
        //this.currentColumn = column;
    },

    onMenuItemClick: function (e) {

        var grid = this.grid;
        var menu = e.sender;
        var columns = grid.getBottomColumns();
        var items = menu.getItems();
        var item = e.item;
        var targetColumn = item._column;
        var currentColumn = this.currentColumn;

        //排序
        var sortField = currentColumn.sortField || currentColumn.field;
        if (item.name === "asc") {
            grid.sortBy(sortField, "asc");
            menu.hide();
            return;
        }

        if (item.name === "desc") {
            grid.sortBy(sortField, "desc");
            menu.hide();
            return;
        }

        //显示/隐藏列
        if (targetColumn) {

            //确保起码有一列是显示的
            var checkedCount = 0;
            var columnsItem = mini.getbyName("showcolumn", menu);
            var childMenuItems = columnsItem.menu.items;

            for (var i = 0, l = childMenuItems.length; i < l; i++) {
                var it = childMenuItems[i];
                if (it.getChecked()) checkedCount++;
            }

            if (checkedCount < 1) {
                item.setChecked(true);
            }

            //显示/隐藏列
            if (item.getChecked()) grid.showColumn(targetColumn);
            else grid.hideColumn(targetColumn);
        }
    }

};

//---------------------------message------------------------------------------//
var Message = {

    loading: function (message, callback) {
        mini.mask({
            el: document.body,
            cls: 'mini-mask-loading',
            html: message
        });

        if (callback && callback()) {
            mini.unmask(document.body);
        }
    },

    alert: function (message, callback) {
        message = message.htmlEncode();
        mini.alert(message, '提示', function (action) {
            if (action === 'ok') {
                callback && callback();
            }
        });
    },

    error: function (message, callback) {
        mini.showMessageBox({
            title: '错误',
            message: '<span style="padding-left:10px;">' + message + '</span>',
            buttons: ['ok'],
            iconCls: 'mini-messagebox-error',
            callback: function (action) {
                if (action === 'ok') {
                    callback && callback();
                }
            }
        });
    },

    confirm: function (message, callback) {
        message = message.htmlEncode();
        mini.confirm(message, '确认提示', function (action) {
            if (action === 'ok') {
                callback && callback();
            }
        });
    },

    prompt: function (label, title, callback) {
        mini.prompt(label, title, function (action, value) {
            if (action === "ok") {
                callback && callback(value);
            }
        });
    },

    promptMulti: function (label, title, callback) {
        mini.prompt(label, title, function (action, value) {
            if (action === "ok") {
                callback && callback(value);
            }
        }, true);
    },

    tips: function (option) {
        if (typeof option === 'string')
            option = { content: option };

        mini.showTips({
            content: option.content,
            state: option.state || 'info',
            x: option.x || 'center',
            y: option.y || 'top',
            timeout: option.timeout || 3000
        });
    },

    notify: function (option) {
        mini.showMessageBox({
            showModal: false,
            width: option.width || 250,
            title: option.title || "提示",
            iconCls: option.iconCls || "mini-messagebox-warning",
            message: option.message,
            timeout: option.timeout || 3000,
            x: option.x || 'right',
            y: option.y || 'bottom'
        });
    },

    result: function (res, callback) {
        if (!res.ok) {
            this.alert(res.message);
            return;
        }

        if (res.message) {
            this.tips(res.message);
        }

        callback && callback(res.data);
    }

};

//---------------------------dialog------------------------------------------//
var Dialog = {

    show: function (option) {
        if (option.id) {
            var win = mini.get(option.id);
            win.show();
            option.callback && option.callback(win);
            return win;
        }

        var dialog = mini.get('dialog');
        dialog.setTitle(option.title);
        dialog.setIconCls(option.iconCls || 'fa-windows');
        dialog.setWidth(option.width || 500);
        dialog.setHeight(option.height || 300);
        dialog.show();

        if (option.max) {
            dialog.max();
        }

        if (option.url) {
            $('#dialog .mini-panel-body').loadHtml(
                option.url,
                option.param,
                function () {
                    mini.parse();
                    option.callback && option.callback(dialog);
                }
            );
        }

        return dialog;
    },

    open: function (option) {
        var win = mini.open({
            url: option.url,
            showMaxButton: true,
            allowResize: true,
            title: option.title,
            width: option.width,
            height: option.height,
            onload: function () {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, 'load');
                }
            },
            ondestroy: function (action) {
                if (option.callback) {
                    var iframe = this.getIFrameEl();
                    option.callback(iframe.contentWindow, action);
                }
            }
        });
        option.max && win.max();
        return win;
    },

    close: function (top = false) {
        if (top) {
            window.CloseOwnerWindow();
        } else {
            mini.get('dialog').hide();
        }
    }

};

//---------------------------toolbar------------------------------------------//
var Toolbar = {

    bind: function (tbId, obj) {
        for (var p in obj) {
            bindButton(tbId, p, obj);
        }

        var top = !tbId.startsWith('tbForm');
        var btnClose = $('#' + tbId + ' #close');
        if (btnClose.length) {
            btnClose.unbind('click').bind('click', function () {
                Dialog.close(top);
            });
        }

        function bindButton(tbId, name, obj) {
            var btn = $('#' + tbId + ' #' + name);
            if (btn.length) {
                btn.unbind('click').bind('click', function () {
                    obj[name].call(obj);
                });
            }
        }
    }

};

//---------------------------grid------------------------------------------//
var Grid = function (name, option) {
    $.extend(true, this.option, option);

    this.name = name;
    this.grid = mini.get('grid' + name);
    this.idField = this.grid.getIdField();

    var _this = this;
    if ($('#query' + name).length) {
        this.query = new Form('query' + name);
        var btnSearch = mini.get('search', this.query);
        if (btnSearch) {
            btnSearch.on('click', function () {
                _this.search();
            });
        }
    }

    var columns = this.grid.getColumns();
    for (var i = 0; i < columns.length; i++) {
        if (columns[i].displayField) {
            this.grid.updateColumn(columns[i], {
                renderer: _this._onColumnRender
            });
        }
    }
};
Grid.prototype = {

    option: {
    },

    _onColumnRender: function (e) {
        var displayField = e.column.displayField;
        if (displayField === 'icon') {
            var value = e.record[e.column.field];
            return '<span class="mini-icon mini-iconfont ' + e.value + '"></span>';
        } else if (displayField.startWith('code.')) {
            var type = displayField.replace('code.', '');
            var code = Code.getCode(type, e.value);
            var text = e.value;
            if (code && code.text) {
                text += '-' + code.text;
            }
            return text;
        } else {
            return e.record[displayField];
        }
    },

    _queryData: function (isLoad, callback) {
        var query = this.query ? this.query.getData(true) : '';
        var grid = this.grid;
        grid.clearSelect(false);
        grid.load(
            { query: query, isLoad: isLoad },
            function (e) {
                if (callback) {
                    callback({ sender: grid, result: e.result });
                }
            },
            function () {
                Message.tips('查询出错！');
            }
        );
        new ColumnsMenu(grid);
    },

    bind: function (type, callback) {
        this.grid.on(type, callback);
    },

    search: function (callback) {
        this._queryData(false, callback);
    },

    load: function (callback) {
        this._queryData(true, callback);
    },

    reload: function () {
        this.grid.reload();
    },

    validate: function (tabsId, tabIndex) {
        this.grid.validate();
        if (this.grid.isValid())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }

        var error = this.grid.getCellErrors()[0];
        this.grid.beginEditCell(error.record, error.column);
        return false;
    },

    getChanges: function (encode) {
        var data = this.grid.getChanges();
        return encode ? mini.encode(data) : data;
    },

    getSelecteds: function (encode) {
        this.grid.accept();
        var data = this.grid.getSelecteds();
        return encode ? mini.encode(data) : data;
    },

    getData: function (encode) {
        var data = this.grid.getData();
        return encode ? mini.encode(data) : data;
    },

    setData: function (data, callback) {
        this.clear();
        if (data) {
            this.grid.setData(data);
            callback && callback(data);
        }
    },

    clear: function () {
        this.grid.setData([]);
    },

    addRow: function (data, index) {
        if (!index) {
            index = this.grid.getData().length;
        }

        this.grid.addRow(data, index);
        this.grid.cancelEdit();
        this.grid.beginEditRow(data);
    },

    updateRow: function (e, data) {
        e.sender.updateRow(e.record, data);
    },

    deleteRow: function (uid) {
        var row = this.grid.getRowByUid(uid);
        if (row) {
            this.grid.removeRow(row);
        }
    },

    checkSelect: function (callback) {
        var rows = this.grid.getSelecteds();
        if (rows.length === 0) {
            Message.tips('请选择一条记录！');
        } else if (rows.length > 1) {
            Message.tips('只能选择一条记录！');
        } else if (callback) {
            callback(rows[0]);
        }
    },

    checkMultiSelect: function (callback) {
        var rows = this.grid.getSelecteds();
        if (rows.length === 0) {
            Message.tips('请选择一条或多条记录！');
        } else if (callback) {
            var data = this.getRowDatas(rows);
            callback(rows, data);
        }
    },

    deleteRows: function (callback) {
        this.checkMultiSelect(function (rows, data) {
            Message.confirm('确定要删除选中的记录？', function () {
                callback && callback(rows, data);
            });
        });
    },

    getRowDatas: function (rows, fields) {
        var datas = [];
        if (fields) {
            $(rows).each(function (i, d) {
                var data = {};
                $(fields).each(function (i, p) {
                    data[p] = d[p] || '';
                });
                datas.push(data);
            });
        } else {
            var id = this.grid.idField;
            $(rows).each(function (i, d) {
                datas.push(d[id] || '');
            });
        }
        return mini.encode(datas);
    },

    hideColumn: function (indexOrName) {
        var column = this.grid.getColumn(indexOrName);
        this.grid.updateColumn(column, { visible: false });
    },

    showColumn: function (indexOrName) {
        var column = this.grid.getColumn(indexOrName);
        this.grid.updateColumn(column, { visible: true });
    },

    setColumns: function (columns) {
        this.grid.setColumns(columns);
    }

};

//---------------------------form------------------------------------------//
var Form = function (formId, option) {
    $.extend(true, this.option, option);

    this.formId = formId;
    this.form = new mini.Form('#' + formId);

    var inputs = this.form.getFields();
    for (var i = 0; i < inputs.length; i++) {
        var input = inputs[i];
        if (input.type === 'combobox' ||
            input.type === 'checkboxlist' ||
            input.type === 'radiobuttonlist') {
            if (input.data.length <= 1) {
                input.setData(Code.getCodes(input.id));
            }
        }
        this[input.id] = input;
    }

    if (this.option.data) {
        this.setData(this.option.data, this.option.callback);
    }
};
Form.prototype = {

    option: {
    },

    reset: function () {
        this.form.reset();
    },

    clear: function (controls) {
        if (controls) {
            var _this = this;
            $(controls.split(',')).each(function (i, c) {
                var control = mini.getbyName(c, _this.form);
                if (control) {
                    control.setValue('');
                    if (control.type === 'autocomplete') {
                        control.setText('');
                    }
                }
            });
        } else {
            this.form.clear();
        }
    },

    validate: function (tabsId, tabIndex) {
        if (this.form.validate())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(tabIndex);
            tabs.activeTab(tab);
        }
        return false;
    },

    getData: function (encode) {
        var data = this.form.getData(true);
        return encode ? mini.encode(data) : data;
    },

    setData: function (data, callback) {
        if (data) {
            this.form.setData(data);
            callback && callback(this, data);
            this.form.setChanged(false);
            this.bindEnterJump();
        }
    },

    saveData: function (option) {
        if (!this.validate(option.tabsId, option.tabIndex))
            return;

        Ajax.postJson(option.url, this.getData(), function (res) {
            Message.result(res, function (data) {
                option.callback && option.callback(data);
            });
        });
    },

    bindEnterJump: function () {
        var inputs = this.form.getFields();
        var activeIndexes = getActiveIndexes(inputs);

        for (var i = 0, len = activeIndexes.length; i < len; i++) {
            (function (i) {
                var index = activeIndexes[i];
                var nextIndex = activeIndexes[i + 1];

                if (i === len - 1) {
                    nextIndex = activeIndexes[0];
                }

                var current = inputs[index];
                $(current.getEl()).keyup(function (e) {
                    if (e.keyCode === 13) {
                        var nextInput = inputs[nextIndex];
                        setTimeout(function () {
                            nextInput.focus();
                            if (nextInput.type !== 'textarea') {
                                nextInput.selectText();
                            }
                        }, 10);
                    } else if (i > 0 && e.keyCode === 38) {
                        var preInput = inputs[activeIndexes[i - 1]];
                        if (current.type !== 'textarea' && (
                            current.type !== 'autocomplete' &&
                            current.type !== 'combobox' ||
                            !current.isShowPopup()
                        )) {
                            setTimeout(function () {
                                preInput.focus();
                                if (preInput.type !== 'textarea') {
                                    preInput.selectText();
                                }
                            }, 10);
                        }
                    }
                });
            })(i);
        }

        function getActiveIndexes(inputs) {
            var indexes = [];
            for (var i = 0, len = inputs.length; i < len; i++) {
                var input = inputs[i];
                $(input.getEl()).unbind('keyup');

                if (input.type !== 'hidden' &&
                    input.type !== 'checkbox' &&
                    input.type !== 'checkboxlist' &&
                    input.type !== 'radiobuttonlist' &&
                    input.type !== 'htmlfile' &&
                    input.getEnabled() === true &&
                    input.getVisible() === true)
                    indexes.push(i);
            }
            return indexes;
        }
    },

    model: function (isLabel) {
        var labelClass = 'form-input-label-model';
        $('span.' + labelClass).remove();
        var inputs = this.form.getFields();
        for (var i = 0, len = inputs.length; i < len; i++) {
            var input = inputs[i];
            input.setVisible(!isLabel);

            if (input.type === 'hidden' || !isLabel)
                continue;

            var text = input.getValue();
            if (input.type === 'combobox' ||
                input.type === 'autocomplete' ||
                input.type === 'listbox' ||
                input.type === 'checkbox' ||
                input.type === 'checkboxlist' ||
                input.type === 'radiobuttonlist' ||
                input.type === 'datepicker' ||
                input.type === 'timespinner') {
                text = input.getText();
            } else if (input.type === 'textarea') {
                text = text.htmlEncode();
            }

            var html = '<span class="' + labelClass + '">' + text + '</span>';
            $(input.getEl()).after(html);
        }
    }

};