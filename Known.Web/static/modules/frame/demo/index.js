var DemoView = {

    show: function () {
        mini.get('leftTree').on('nodeselect', this.onTreeNodeSelect);
    },

    close: function () {
        window.CloseOwnerWindow();
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

$(function () {
    mini.parse();
    DemoView.show();
});