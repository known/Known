var DemoGrid = {
    gridId: 'Demo',
    show: function () {
        var _this = this;
        mini.get('btnRefresh').on('click', function () {
            Grid.reload(_this.gridId);
        });
        mini.get('btnClose').on('click', function () {
            DemoView.close();
        });
        mini.get('btnSearch').on('click', function () {
            Grid.search(_this.gridId);
        });

        Grid.load(_this.gridId);
    }
};