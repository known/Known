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

function Dashboard() {
    var board, _content;
    var navItems = [
        {
            text: '站点地图', icon: 'fa fa-sitemap', component: function () {
                return new MySiteMap();
            }, isDefault: true
        },
        {
            text: '仪表盘', icon: 'fa fa-tachometer', component: function () {
                return _getDashboard();
            }
        }
    ];

    //methods
    this.render = function (dom) {
        var elem = $('<div>').addClass('fit').css('padding-top', '10px').appendTo(dom);
        //var nav = $('<ul>').addClass('dash-nav').appendTo(elem);
        //_content = $('<div>').addClass('dash-content').appendTo(elem);
        //_createNavbar(nav);
        board = _getDashboard();
        board.render(elem);
    }

    this.mounted = function () {
        board.mounted();
    }

    //private
    function _getDashboard() {
        var page = Page[appId + 'Dashboard'];
        var dashboard = page && page.component
            ? page.component
            : new MyDashboard();

        //if (curUser.UserName === 'System') {
        //    var option = dashboard.sysOption || {};
        //    dashboard = new SysDashboard(option);
        //}

        return dashboard;
    }

    function _createNavbar(nav) {
        for (var i = 0; i < navItems.length; i++) {
            var item = navItems[i];
            var li = _createNavItem(nav, item);
            if (item.isDefault) {
                li.click();
            }
        }
    }

    function _createNavItem(nav, item) {
        var li = $('<li>').data('item', item).appendTo(nav).on('click', function () {
            $('.dash-nav li').removeClass('active');
            $(this).addClass('active');
            var obj = $(this).data('item');
            _content.html('');
            obj.component().render(_content);
        });
        $('<i>').addClass(item.icon).appendTo(li);
        $('<span>').html(item.text).appendTo(li);
        return li;
    }

    function MyDashboard() {
        //fields
        var notice, shortcut, commlink;
        var chartFlow = new ChartFlow();
        var chartBrowser = new ChartBrowser();
        var icons = [
            {
                name: '统计1', icon: 'fa fa-th-list', value: 20, width: '25%', style: { backgroundColor: '#00c0ef' }, onClick: function () {
                    alert('统计');
                }
            },
            {
                name: '统计2', icon: 'fa fa-product-hunt', value: 10, width: '25%', style: { backgroundColor: '#00a65a' }, targetPage: {
                    id: 'test',
                    title: '测试',
                    icon: 'fa fa-product-hunt',
                    type: 'page',
                    code: 'Test'
                }
            },
            { name: '统计3', icon: 'fa fa-newspaper-o', value: 5, width: '25%', style: { backgroundColor: '#dd4b39' }, targetPage: 'SysDictionary' },
            { name: '统计4', icon: 'fa fa-commenting-o', value: 0, width: '25%', style: { backgroundColor: '#f0ad4e' } }
        ];

        //methods
        this.sysOption = { icons: icons };

        this.render = function (dom) {
            _createLeft(dom);
            _createRight(dom);
        }

        this.mounted = function () {
            notice.load();
            shortcut.load();
            commlink.load();
            chartFlow.load();
            chartBrowser.load();
        }

        //private
        function _createLeft(elem) {
            var left = _createCol(elem, '75%');

            //left row1
            var row11 = $('<div>').addClass('row').appendTo(left);
            var iconRow = $('<div>').addClass('card').appendTo(row11);
            Dashboard.createIconCards(iconRow, icons);

            //left row2
            var row12 = $('<div>').addClass('row').appendTo(left);
            Dashboard.createTodoList(row12, { height: '200px' });
            notice = Dashboard.createNotice(row12, { height: '200px' });

            //left row3
            var row13 = $('<div>').addClass('row').appendTo(left);
            var col131 = _createCol(row13, '50%');
            chartFlow.render(col131);
            var col132 = _createCol(row13, '50%');
            chartBrowser.render(col132);
        }

        function _createRight(elem) {
            var right = _createCol(elem, '25%');

            //right row1
            var row21 = _createRow(right);
            shortcut = Dashboard.createShortcut(row21, { height: '351px' });

            //right row2
            var row22 = _createRow(right);
            commlink = Dashboard.createCommLink(row22, { height: '300px' });
        }

        function _createRow(right) {
            return $('<div>').addClass('row').css('padding-left', '0').appendTo(right);
        }

        function _createCol(dom, width) {
            return $('<div>').css({ float: 'left', width: width }).appendTo(dom);
        }
    }

    function ChartFlow() {
        this.render = function (dom) {
            new Card({
                icon: 'fa fa-line-chart', title: '分析1', style: { marginRight: '5px' },
                body: '<div id="chartFlow" style="height:300px;">'
            }).render().appendTo(dom);
        }

        this.load = function () {
            var chart = new Chart('chartFlow');
            chart.setOption({
                title: { left: 'center', text: '' },
                tooltip: { trigger: 'axis' },
                legend: { data: ['PV', 'UV'] },
                xAxis: {
                    splitLine: { show: false },
                    data: ["06:00", "06:30", "07:00", "07:30", "08:00", "08:30", "09:00", "09:30", "10:00", "11:30", "12:00", "12:30", "13:00", "13:30", "14:00", "14:30", "15:00", "15:30", "16:00", "16:30", "17:00", "17:30", "18:00", "18:30", "19:00", "19:30", "20:00", "20:30", "21:00", "21:30", "22:00", "22:30", "23:00", "23:30"]
                },
                yAxis: {},
                series: [{
                    name: "PV",
                    type: "line",
                    data: [111, 222, 333, 444, 555, 666, 3333, 33333, 55555, 66666, 33333, 3333, 6666, 11888, 26666, 38888, 56666, 42222, 39999, 28888, 17777, 9666, 6555, 5555, 3333, 2222, 3111, 6999, 5888, 2777, 1666, 999, 888, 777]
                }, {
                    name: "UV",
                    type: "line",
                    data: [11, 22, 33, 44, 55, 66, 333, 3333, 5555, 12666, 3333, 333, 666, 1188, 2666, 3888, 6666, 4222, 3999, 2888, 1777, 966, 655, 555, 333, 222, 311, 699, 588, 277, 166, 99, 88, 77]
                }]
            });
        }
    }

    function ChartBrowser() {
        this.render = function (dom) {
            new Card({
                icon: 'fa fa-pie-chart', title: '分析2', style: { marginLeft: '5px' },
                body: '<div id="chartBrowser" style="height:300px;">'
            }).render().appendTo(dom);
        }

        this.load = function () {
            var chart = new Chart('chartBrowser');
            chart.setOption({
                title: { left: 'center', text: '' },
                tooltip: { trigger: 'item' },
                legend: { data: ["Chrome", "Firefox", "IE 8.0", "Safari", "其它浏览器"] },
                series: [{
                    "name": "访问来源",
                    "type": "pie",
                    "radius": "55%",
                    "center": ["50%", "50%"],
                    "data": [
                        { "value": 9052, "name": "Chrome" },
                        { "value": 1610, "name": "Firefox" },
                        { "value": 3200, "name": "IE 8.0" },
                        { "value": 535, "name": "Safari" },
                        { "value": 1700, "name": "其它浏览器" }
                    ]
                }]
            });
        }
    }
}

Dashboard.createIconCards = function (container, items) {
    if (items && items.length > 0) {
        for (var i = 0; i < items.length; i++) {
            new IconCard(items[i]).render().appendTo(container);
        }
    }
}

Dashboard.createTodoList = function (container, option) {
    var col = $('<div>').css({ float: 'left', width: '50%' }).appendTo(container);
    new Card({
        icon: 'fa fa-tasks', title: Language.PeddingTask, style: { marginRight: '5px' },
        callback: function (e) {
            e.body.css('height', option.height);
            e.body.html('<div id="todoList">');
        }
    }).render().appendTo(col);
    BizFlow.loadTodos('todoList');
}

Dashboard.createNotice = function (container, option) {
    var col = $('<div>').css({ float: 'left', width: '50%' }).appendTo(container);
    var notice;
    new Card({
        icon: 'fa fa-bell-o', title: Language.Notice, style: { marginLeft: '5px' },
        tool: '<span class="toolNotice">' + Language.More + '</span>',
        callback: function (e) {
            e.body.css('height', option.height);
            e.body.html('<div id="noticeList">');
            e.header.find('.toolNotice').on('click', function () { notice.more(); });
        }
    }).render().appendTo(col);
    notice = new NoticeList('noticeList');
    return notice;
}

Dashboard.createShortcut = function (container, option) {
    var shortcut;
    new Card({
        icon: 'fa fa-external-link', title: Language.Shortcut,
        tool: '<span class="toolShortcut">' + Language.Set + '</span>',
        callback: function (e) {
            e.body.css('height', option.height);
            e.body.html('<ul id="ulShortcut">');
            e.header.find('.toolShortcut').on('click', function () { shortcut.add(); });
        }
    }).render().appendTo(container);
    shortcut = new UserLink('ulShortcut', 'shortcut');
    return shortcut;
}

Dashboard.createCommLink = function (container, option) {
    var commlink;
    new Card({
        icon: 'fa fa-external-link', title: Language.CommonLink,
        tool: '<span class="toolCommlink">' + Language.Add + '</span>',
        callback: function (e) {
            e.body.css('height', option.height);
            e.body.html('<ul id="ulLink">');
            e.header.find('.toolCommlink').on('click', function () { commlink.add(); });
        }
    }).render().appendTo(container);
    commlink = new UserLink('ulLink', 'commlink');
    return commlink;
}

$.extend(Page, {
    Dashboard: { component: new Dashboard() }
});