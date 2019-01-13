var MainTabs = {

    tabsId: 'mainTabs',

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

    index: function () {
        var tab = this.active({ id: 'index' });
        $(tab.bodyEl).loadHtml('/Home/Partial', {
            name: 'Dashboard'
        }, function () {
            Dashboard.show();
        });
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