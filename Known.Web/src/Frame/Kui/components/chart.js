Chart.isLoad = false;
function Chart(id) {
    if (!Chart.isLoad) {
        Utils.addJs('/libs/echarts.min.js');
        Chart.isLoad = true;
    }

    //field
    var _chart = echarts.init(document.getElementById(id));

    //method
    this.clear = function () {
        _chart.clear();
    }

    this.setOption = function (option) {
        _chart.setOption(option);
    }

    this.setLine = function (option) {
        _chart.setOption(option);
    }

    this.setPie = function (option) {
        _chart.setOption(option);
    }
}