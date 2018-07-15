var DemoView = {
    element: {},
    show: function () {
        this.element = {
            tree: mini.get('leftTree')
        };
        this.element.tree.on('nodeselect', this.onTreeNodeSelect);
    },
    onTreeNodeSelect: function (e) {
        $('#page').loadHtml('/frame/partial', {
            name: e.node.view
        }, function () {
            mini.parse();
        });
    }
};