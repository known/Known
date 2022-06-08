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

app.setTabbar([
    { icon: 'fa fa-home', id: 'tbHome', name: '首页', hideTop: true, component: new DevHome() },
    { icon: 'fa fa-wifi', id: 'tbOnline', name: '监测', title: '在线监测', component: new IoTOnline() },
    //{ icon: 'fa fa-cog', id: 'tbSetting', name: '设置', title: '参数设置', component: new IoTSetting() },
    { icon: 'fa fa-user-o', id: 'tbUser', name: '我的', title: '我的信息', component: new SysUser() },
]);
app.loadUser(baseUrl + '/Home/GetUserData');
$('#tbUser .badge').html(3).show();

function DevHome() {
    //fields
    var slider = new Slider({
        height: '35%',
        items: [
            { img: '/img/bg.jpg' }
        ]
    });

    var menu = new Menu({
        lineCount: 4,
        items: [
            { bgColor: '#66cc66', id: 'menuForm', icon: 'fa fa-wpforms', name: '表单示例', component: new DevForm() },
            { bgColor: '#224b39', id: 'menuGrid', icon: 'fa fa-table', name: '表格示例', component: new DevGrid() },
            { bgColor: '#0096dd', id: 'menuOnline', icon: 'fa fa-wifi', name: '实时监测', component: new IoTOnline() },
            { bgColor: '#9b59b6', id: 'menuSetting', icon: 'fa fa-cog', name: '参数设置', component: new IoTSetting() },
            { bgColor: '#dd4b39', id: 'menuError', icon: 'fa fa-close', name: '错误页面', component: new DevError() },
            { bgColor: '#00a0ff', id: 'menuMap', icon: 'fa fa-map-marker', name: '地图示例' },
            { bgColor: '#eecc00', id: 'menuChart', icon: 'fa fa-bar-chart', name: '图表示例' },
            { bgColor: '#00a65a', id: 'menuAdd', icon: 'fa fa-plus', name: '功能待加' }
        ]
    });

    //methods
    this.render = function () {
        var elem = $('<div class="content">');
        slider.render().appendTo(elem);
        $('<h2>').addClass('sys-title').html('Known移动端UI系统').appendTo(elem);
        menu.render().appendTo(elem);
        return elem;
    }

    this.mounted = function () {
    }
}

function DevOnline() {
    var tabs = new Tabs({
        items: [
            { name: '实时监测' },
            { name: '地图监测' },
            { name: '实时报警' },
            { name: '视频监控' }
        ]
    });

    this.render = function () {
        var elem = $('<div class="content">');
        tabs.render().appendTo(elem);
        return elem;
    }
}