var Index = {

    mainTabs: null,

    show: function () {
        this.mainTabs = this._getMainTabs();
        this._initLeftTree();
    },

    _getMainTabs: function () {
        return $('#mainTabs').tabs({
            border: false,
            onSelect: function (title) {

            }
        });
    },

    _initLeftTree: function () {
        var mainTabs = this.mainTabs;

        $('#leftTree').tree({
            method: 'get',
            url: 'static/data/menu.json',
            onClick: function (node) {
                if (node.children)
                    return;

                var tab = mainTabs.tabs('getTab', node.text);
                if (tab) {
                    var index = mainTabs.tabs('getTabIndex', tab);
                    mainTabs.tabs('select', index);
                } else {
                    mainTabs.tabs('add', {
                        id: node.id,
                        title: node.text,
                        content: 'Body' + node.text,
                        closable: true
                    });
                }
            }
        });
    }

};

Index.show();