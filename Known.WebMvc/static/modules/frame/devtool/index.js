var DemoView = {

    show: function () {
        mini.get('leftTree').on('nodeselect', this.onTreeNodeSelect);
    },

    close: function () {
        window.CloseOwnerWindow();
    },

    onTreeNodeSelect: function (e) {
        $('#splitRight').loadHtml('/Frame/Partial', {
            name: e.node.view
        }, function () {
            mini.parse();
            if (e.node.id === 'code') {
                DevCodeGen.show();
            } else if (e.node.id === 'data') {
                DevDatabase.show();
            } else if (e.node.id === 'grid') {
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