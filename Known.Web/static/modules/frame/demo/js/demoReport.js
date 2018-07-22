var DemoReport = {

    grid: null,
    toolbar: null,

    show: function () {
        this.grid = new Grid('Report');
        this.toolbar = new Toolbar('tbReport', this);

        this.grid.load();
    }

};