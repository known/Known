var ModuleView = {

    leftTree: null,

    show: function () {
        this.leftTree = mini.get('leftTree');
        this.loadTree();
        this.leftTree.on('nodeselect', this.onTreeNodeSelect);
    },

    loadTree: function () {
        var _this = this;
        Ajax.getJson('/api/module/gettreedatas', function (result) {
            _this.leftTree.loadData(result.Data);
        });
    },

    onTreeNodeSelect: function (e) {
        ModuleGrid.show({
            moduleId: e.node.id,
            callback: ModuleView.loadTree
        });
    }

};

$(function () {
    mini.parse();
    ModuleView.show();
});