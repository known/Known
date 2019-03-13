//Menu
//{ id, text, parentId?, href?, hrefTarget?, icon, iconCls, cls, expanded, children }

var Menu_Id = 1;

var Menu = function (element, options) {
    this.element = $(element);
    this.options = $.extend(true, {}, this.options, options);
    this.init();
};
Menu.prototype = {

    options: {
        data: null,
        itemclick: null
    },

    loadData: function (data) {
        this.options.data = data || [];
        this.refresh();
    },

    refresh: function () {
        this._render();
    },

    init: function () {
        var me = this,
            opt = me.options,
            el = me.element;

        //el.addClass('menu');

        me.loadData(opt.data);

        el.on('click', '.menu-title', function (event) {
            var el = $(event.currentTarget);

            var li = el.parent();

            var item = me.getItemByEvent(event);

            //alert(item);
            //me.toggleItem(item);

            li.toggleClass('open');

            if (opt.itemclick) opt.itemclick.call(me, item);

        });

    },

    _render: function () {
        var data = this.options.data || [];
        var html = this._renderItems(data, null);
        this.element.html(html);
    },

    _renderItems: function (items, parent) {
        var s = '<ul class="' + (parent ? "menu-submenu" : "menu") + '">';
        for (var i = 0, l = items.length; i < l; i++) {
            var item = items[i];
            s += this._renderItem(item);
        }
        s += '</ul>';
        return s;
    },

    _renderItem: function (item) {

        var me = this,
            hasChildren = item.children && item.children.length > 0;

        var s = '<li class="' + (hasChildren ? 'has-children' : '') + (item.expanded ? ' open' : '') + '">';        //class="menu-item" open, expanded?

        s += '<a class="menu-title" data-id="' + item.id + '" ';
        //if (item.href) {
        //   s += 'href="' + item.href + '" target="' + (item.hrefTarget || '') + '"';
        //}
        s += '>';

        s += '<i class="menu-icon fa ' + item.iconCls + '"></i>';
        s += '<span class="menu-text">' + item.text + '</span>';

        if (hasChildren) {
            s += '<span class="menu-arrow fa"></span>';
        }

        s += '</a>';

        if (hasChildren) {
            s += me._renderItems(item.children, item);
        }

        s += '</li>';
        return s;
    },

    getItemByEvent: function (event) {
        var el = $(event.target).closest('.menu-title');
        var id = el.attr("data-id");
        return this.getItemById(id);
    },

    getItemById: function (id) {
        var me = this,
            idHash = me._idHash;

        if (!idHash) {
            idHash = me._idHash = {};
            each(me.options.data);
        }

        function each(items) {
            for (var i = 0, l = items.length; i < l; i++) {
                var item = items[i];
                if (item.children) each(item.children);
                idHash[item.id] = item;
            }
        }

        return me._idHash[id];
    }

};

var MenuTip = function (menu) {
    var template = '<div class="tooltip right menutip in"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>';
    var tip = $(template).appendTo(document.body);
    tip.hide();

    menu.element.on("mouseenter", ".menu-title", function (event) {
        if (!$("body").hasClass("compact")) return;

        var jq = $(event.currentTarget);
        var offset = jq.offset(),
            width = jq.outerWidth(),
            height = jq.outerHeight(),
            text = jq.text();

        tip.find(".tooltip-inner").html(text);
        tip.show();

        var tipWidth = tip.outerWidth(),
            tipHeight = tip.outerHeight();

        tip.css({ top: offset.top + height / 2 - tipHeight / 2, left: offset.left + width });

    });

    menu.element.on("mouseleave", ".menu-title", function (event) {
        tip.hide();
    });
};

var Navbar = {

    devTool: function () {
        MainTabs.active({
            id: 'devTool', iconCls: 'fa-puzzle-piece',
            text: '开发工具', url: '/Develop/DevelopView'
        });
    },

    todo: function () {
        MainTabs.active({
            id: 'todo', iconCls: 'fa-tasks',
            text: '代办事项', url: '/System/TodoView'
        });
    },

    cache: function () {
        Ajax.getJson('/User/GetCodes', function (data) {
            Code.setData(data);
            Message.tips({ content: '刷新成功！' });
        });
    },

    info: function () {
        Dialog.show({
            title: '用户信息', iconCls: 'fa-user',
            url: '/Home/Partial', param: { name: 'UserInfo' },
            callback: function () {
                Ajax.getJson('/User/GetUserInfo', function (data) {
                });
                Toolbar.bind('tbFormUpdatePwd', {
                    save: function () {
                        Message.tips('保存成功！');
                    }
                });
            }
        });
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/signout', function () {
                location = location;
            });
        });
    }

};

var Dashboard = {

    show: function () {
        this.buildChart();
    },

    buildChart: function () {
        var chart = new Highcharts.Chart({
            chart: {
                renderTo: 'chartLine',
                type: 'line',
                marginRight: 130,
                marginBottom: 25
            },
            title: {
                text: 'Monthly Average Temperature',
                x: -20 //center
            },
            subtitle: {
                text: 'Source: WorldClimate.com',
                x: -20
            },
            xAxis: {
                categories: ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun',
                    'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec']
            },
            yAxis: {
                title: {
                    text: 'Temperature (°C)'
                },
                plotLines: [{
                    value: 0,
                    width: 1,
                    color: '#808080'
                }]
            },
            tooltip: {
                formatter: function () {
                    return '<b>' + this.series.name + '</b><br/>' +
                        this.x + ': ' + this.y + '°C';
                }
            },
            legend: {
                layout: 'vertical',
                align: 'right',
                verticalAlign: 'top',
                x: -10,
                y: 100,
                borderWidth: 0
            },
            series: [{
                name: 'Tokyo',
                data: [7.0, 6.9, 9.5, 14.5, 18.2, 21.5, 25.2, 26.5, 23.3, 18.3, 13.9, 9.6]
            }, {
                name: 'New York',
                data: [-0.2, 0.8, 5.7, 11.3, 17.0, 22.0, 24.8, 24.1, 20.1, 14.1, 8.6, 2.5]
            }, {
                name: 'Berlin',
                data: [-0.9, 0.6, 3.5, 8.4, 13.5, 17.0, 18.6, 17.9, 14.3, 9.0, 3.9, 1.0]
            }, {
                name: 'London',
                data: [3.9, 4.2, 5.7, 8.5, 11.9, 15.2, 17.0, 16.6, 14.2, 10.3, 6.6, 4.8]
            }]
        });
    }

};

var MainTabs = {

    tabsId: 'mainTabs',

    init: function () {
        var tab = this.active({ id: 'index' });
        $(tab.bodyEl).loadHtml('/Home/Partial', {
            name: 'Dashboard'
        }, function () {
            Dashboard.show();
        });
    },

    active: function (item) {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getTab(item.id);
        if (!tab) {
            tab = tabs.addTab({
                name: item.id, title: item.text, url: item.url,
                iconCls: item.iconCls, showCloseButton: true
            });
        }
        tabs.activeTab(tab);
        tab.bodyEl = tabs.getTabBodyEl(tab);
        return tab;
    },

    home: function () {
        this.active({ id: 'index' });
    },

    refresh: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        tabs.reloadTab(tab);
    },

    remove: function () {
        var tabs = mini.get(this.tabsId);
        var tab = tabs.getActiveTab();
        if (tab.name !== 'index') {
            tabs.removeTab(tab);
        }
    },

    fullScreen: function () {
    }

};

$(function () {
    $('#wrapper').show();

    //menu
    var menu = new Menu('#mainMenu', {
        itemclick: function (item) {
            if (!item.children) {
                MainTabs.active(item);
            }
        }
    });

    $('.sidebar').mCustomScrollbar({ autoHideScrollbar: true });

    new MenuTip(menu);

    Ajax.getJson('/User/GetModules', function (result) {
        menu.loadData(result.menus);
        Code.setData(result.codes);
    });

    //toggle
    $('#toggle, .sidebar-toggle').click(function () {
        var body = $('body'), toggle = $('.sidebar-toggle i');
        body.toggleClass('compact');
        if (body.hasClass('compact')) {
            toggle.removeClass('fa-dedent').addClass('fa-indent');
        } else {
            toggle.removeClass('fa-indent').addClass('fa-dedent');
        }
        mini.layout();
    });

    //dropdown
    $('.dropdown-toggle').click(function (event) {
        $(this).parent().addClass('open');
        return false;
    });
    $(document).click(function (event) {
        $('.dropdown').removeClass('open');
    });

    mini.parse();

    Toolbar.bind('tbNavbar', Navbar);
    Toolbar.bind('tbMainTabs', MainTabs);
    MainTabs.init();

});