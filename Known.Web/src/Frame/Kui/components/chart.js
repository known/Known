/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

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