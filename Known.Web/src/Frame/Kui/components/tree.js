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

function Tree(elem, option) {
    if (!$.fn.zTree) {
        Utils.addCss('/libs/ztree/ztree.css');
        Utils.addJs('/libs/ztree/jquery.ztree.min.js');
    }

    //field
    var _simpleData = option.simpleData === undefined ? true : option.simpleData,
        _autoLoad = option.autoLoad === undefined ? true : option.autoLoad;
    var _elem = typeof elem === 'string' ? $('#' + elem) : elem,
        _id = typeof elem === 'string' ? elem : elem.attr('id'),
        _this = this,
        _tree,
        _openNodeIds = [];

    //init
    if (_elem.length) {
        _init();
    }

    //property
    this.id = _id;
    this.list = [];
    this.data = [];
    this.selectedNode = null;

    //method
    this.render = function () {
        _elem = $('<ul>').attr('id', _id);
        _init();
        return _elem;
    }

    this.setData = function (data) {
        _setData(data);
    }

    this.reload = function (where) {
        _load(true, where);
    }

    this.getCheckedNodes = function () {
        return _tree.getCheckedNodes(true);
    }

    this.getNodeData = function (filter) {
        var items = this.list.filter(filter);
        if (items.length)
            return items[0].data;

        return null;
    }

    //private
    function _init() {
        _elem.addClass('ztree');

        if (option.data) {
            _setData(option.data);
        }

        if (option.url && _autoLoad) {
            _load(false, null);
        }
    }

    function _load(isReload, where) {
        if (!option.url)
            return;

        $.get(option.url, where, function (result) {
            if (isReload) {
                for (var i = 0; i < result.length; i++) {
                    var index = _openNodeIds.indexOf(result[i].id);
                    if (index > -1) {
                        result[i].open = true;
                    }
                }
            }
            _setData(result);
            option.onLoad && option.onLoad(_this.data);
        });
    }

    function _setData(data) {
        var treeOption = {
            view: {
                nodeClasses: function (t, n) {
                    return { add: ['tree-' + (n.type || 'type')] };
                }
            },
            data: { key: { url: '' } },
            callback: {
                onClick: function (e, id, node) {
                    _this.selectedNode = node;
                    option.onClick && option.onClick(node);
                },
                onExpand: function (e, id, node) {
                    _openNodeIds.push(node.id);
                },
                onCollapse: function (e, id, node) {
                    var index = _openNodeIds.indexOf(node.id);
                    if (index > -1) {
                        _openNodeIds.splice(index, 1);
                    }
                }
            }
        };

        if (option.check) {
            treeOption.check = {
                enable: true,
                chkboxType: { 'Y': 'ps', 'N': 's' }
            };
        }

        if (_simpleData) {
            treeOption.data.simpleData = { enable: true, pIdKey: option.parentKey || 'pid' };
        }

        _this.list = data;

        if (option.dataFilter) {
            var treeData = option.dataFilter(data);
            _tree = $.fn.zTree.init(_elem, treeOption, treeData);
        } else {
            _tree = $.fn.zTree.init(_elem, treeOption, data);
        }

        if (option.expandAll) {
            _tree.expandAll(true);
        }

        _this.data = _tree.getNodes();
    }
}