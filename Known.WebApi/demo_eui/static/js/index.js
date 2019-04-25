$.fn.panel.defaults.loadingMessage = '加载中...';
$.fn.datagrid.defaults.loadMsg = '数据加载中，请稍等...';
$.fn.datagrid.defaults.pageList = [10, 20, 30, 40, 50, 100, 200, 500];
$.fn.pagination.defaults.displayMsg = '共 {total} 条数据';
$.fn.pagination.defaults.beforePageText = '第';
$.fn.pagination.defaults.afterPageText = '/{pages}页';

var Index = {

    mainTabs: null,
    leftTree: null,

    show: function () {
        this.mainTabs = this._getMainTabs();
        this.leftTree = this._getLeftTree();
    },

    _getMainTabs: function () {
        var _this = this;
        return $('#mainTabs').tabs({
            border: false,
            onSelect: function (title, index) {
                var leftTree = _this.leftTree;
                if (leftTree) {
                    var node = leftTree.tree('find', { text: title });
                    if (node) {
                        leftTree.tree('expandTo', node.target)
                            .tree('select', node.target);
                    }
                }
            }
        });
    },

    _getLeftTree: function () {
        var _this = this;
        return $('#leftTree').tree({
            animate: true,
            method: 'get',
            url: 'static/data/menu.json',
            onClick: function (node) {
                if (node.children)
                    return;

                var mainTabs = _this.mainTabs;
                if (mainTabs.tabs('exists', node.text)) {
                    mainTabs.tabs('select', node.text);
                } else {
                    mainTabs.tabs('add', {
                        id: node.id,
                        title: node.text,
                        iconCls: node.iconCls,
                        href: '/Pages' + node.url,
                        closable: true,
                        bodyCls: 'content'
                    });
                }
            },
            onLoadSuccess: function (node, data) {
                _this._removeNodeIcon();
            },
            onExpand: function (node) {
                _this._removeNodeIcon();
            }
        });
    },

    _removeNodeIcon: function () {
        $('.fa').removeClass('tree-icon tree-file');
        $('.fa').removeClass('tree-icon tree-folder tree-folder-open tree-folder-closed');
    }

};

Index.show();