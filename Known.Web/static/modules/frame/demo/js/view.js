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
            if (e.node.id === 'grid') {
                DemoGrid.show();
            } else if (e.node.id === 'tab') {
                DemoTab.show();
            } else if (e.node.id === 'form') {
                DemoForm.show();
            } else if (e.node.id === 'report') {
                DemoReport.show();
            }
        });
    }
};