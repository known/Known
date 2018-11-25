var ModuleView = {

    tree: null,

    show: function () {
        this.tree = mini.get('leftTree');
        this.tree.on('nodeselect', this.onTreeNodeSelect);
    },

    loadTree: function () {
        this.tree.load('/api/module/gettreedatas');
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