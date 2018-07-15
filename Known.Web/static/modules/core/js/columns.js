
var ColumnsMenu = function (grid, options) {

    var me = this;
    this.grid = grid;
    this.menu = this.createMenu();
    this.currentColumn = null;

    this.menu.on("beforeopen", this.onBeforeOpen, this);
    //grid.setHeaderContextMenu(this.menu);

    // 
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
}

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
            if (item.text == "&nbsp;") {
                if (column.type == "indexcolumn") item.text = "序号";
                if (column.type == "checkcolumn") item.text = "选择";
            }
            item.enabled = column.enabled;
            item._column = column;
            columnMenuItems.children.push(item);
        }
        items.push(columnMenuItems);

        //        items.push('-');
        //        items.push({ text: "过滤", name: "filter" });
        //        items.push({ text: "取消过滤", name: "clearfilter" });

        menu.setItems(items);

        menu.on("itemclick", this.onMenuItemClick, this);


        $(menu.el).addClass("mini-menu-open");


        return menu;
    },
    onBeforeOpen: function (e) {
        //        var grid = this.grid;
        //        var column = grid.getColumnByEvent(e.htmlEvent);
        //        this.currentColumn = column;
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
        if (item.name == "asc") {
            grid.sortBy(sortField, "asc");
            menu.hide();
            return
        }
        if (item.name == "desc") {
            grid.sortBy(sortField, "desc");
            menu.hide();
            return
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



