var ModuleView = {

    tree: null,

    show: function () {
        this.tree = mini.get('leftTree');
        this.tree.on('nodeselect', this.onTreeNodeSelect);
        this.showGrid('0');
    },

    showGrid: function (pid) {
        var _this = this;
        ModuleGrid.show({
            pid: pid,
            callback: function () {
                _this.tree.reload();
            }
        });
    },

    onTreeNodeSelect: function (e) {
        ModuleView.showGrid(e.node.id);
    }

};

$(function () {
    mini.parse();
    ModuleView.show();
});