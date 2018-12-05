var DemoReport = {

    grid: null,

    show: function () {
        new Toolbar('tbReport', this);

        this.grid = new Grid('Report');
        this.grid.load();
    }

};