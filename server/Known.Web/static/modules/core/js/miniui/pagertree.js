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

    showTreeIcon: true,                 //节点图标    
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