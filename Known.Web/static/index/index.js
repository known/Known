
//{ id, text, parentId?, href?, hrefTarget?, icon, iconCls, cls, expanded, children }

var Menu_Id = 1;

var MainMenu = function (element, options) {
    this.element = $(element);
    this.options = $.extend(true, {}, this.options, options);
    this.init();
};

MainMenu.prototype = {

    options: {
        data: null,
        itemClick: null
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

        el.on('click', '.main-menu-title', function (event) {
            var el = $(event.currentTarget);

            var li = el.parent();

            var item = me.getItemByEvent(event);

            //alert(item);
            //me.toggleItem(item);

            li.toggleClass('open');

            if (opt.itemClick) opt.itemClick.call(me, item);

        });

    },

    _render: function () {
        var data = this.options.data || [];
        var html = this._renderItems(data, null);
        this.element.html(html);
    },

    _renderItems: function (items, parent) {
        var s = '<ul class="' + (parent ? "main-menu-submenu" : "main-menu") + '">';
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

        var s = '<li class="' + (hasChildren ? 'has-children' : '') + '">';        //class="main-menu-item" open, expanded?

        s += '<a class="main-menu-title" data-id="' + item.id + '" ';
        //        if (item.href) {
        //            s += 'href="' + item.href + '" target="' + (item.hrefTarget || '') + '"';
        //        }
        s += '>';


        if (item.image) {
            s += '<i class="main-menu-icon"><img src="' + item.image + '"/></i>';
        } else {
            s += '<i class="main-menu-icon fa ' + item.iconCls + '"></i>';
        }
        s += '<span class="main-menu-text">' + item.text + '</span>';

        if (hasChildren) {
            s += '<span class="main-menu-arrow fa"></span>';
        }

        s += '</a>';

        if (hasChildren) {
            s += me._renderItems(item.children, item);
        }

        s += '</li>';
        return s;
    },

    getItemByEvent: function (event) {
        var el = $(event.target).closest('.main-menu-title');
        var id = el.attr("data-id");
        return this.getItemById(id);
    },

    getItemById: function (id) {
        var me = this,
            idHash = me._idHash;

        if (!idHash) {
            idHash = me._idHash = {};
            function each(items) {
                for (var i = 0, l = items.length; i < l; i++) {
                    var item = items[i];
                    if (item.children) each(item.children);
                    idHash[item.id] = item;
                }
            }
            each(me.options.data);
        }

        return me._idHash[id];
    }

};

var MainMenuTip = function (menu) {
    var template = '<div class="tooltip right menutip in"><div class="tooltip-arrow"></div><div class="tooltip-inner"></div></div>';
    var tip = $(template).appendTo(document.body);
    tip.hide();

    menu.element.on("mouseenter", ".main-menu-title", function (event) {
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
    menu.element.on("mouseleave", ".main-menu-title", function (event) {
        tip.hide();
    });

};

var MainView = {

    mainTabs: null,
    currentTab: null,

    show: function (option) {
        this.mainTabs = $('#mainTabs');
        this._initMenu(option.mainMenuUrl);
        
        var _this = this;
        $('#toggle, .sidebar-toggle').click(function () {
            $('body').toggleClass('compact');
            _this.mainTabs.tabs('resize');
        });
    },

    addTab: function (item) {
        this.currentTab = item;
        if (this.mainTabs.tabs('exists', item.text)) {
            this.mainTabs.tabs('select', item.text);
        } else {
            var url = 'layout.html?p=' + encodeURI(item.url);
            var content = '<iframe src="' + url + '" style="width:100%;height:100%;border:0;"></iframe>';
            this.mainTabs.tabs('add', {
                title: item.text,
                iconCls: item.iconCls,
                content: content,
                closable: true
            });
        }
    },

    openTab: function (obj, url) {
        this.addTab({
            text: $(obj).text(),
            iconCls: $(obj).find('i').attr('class'),
            url: url
        });
    },

    updPwd: function (obj) {
        Message.show();
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function (result) {
            if (result) {
                location = 'login.html';
            }
        });
    },

    _initMenu: function (url) {
        var _this = this;
        var menu = new MainMenu('#mainMenu', {
            itemClick: function (item) {
                if (!item.children) {
                    _this.addTab(item);
                }
            }
        });
        $('.sidebar').mCustomScrollbar({ autoHideScrollbar: true });
        new MainMenuTip(menu);
        $.ajax({
            url: url,
            success: function (data) {
                menu.loadData(data);
            }
        });
    }

};

$(function () {
    MainView.show({
        mainMenuUrl: 'data/menu.json'
    });
});