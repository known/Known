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

function Tabs(option) {
    //fields
    var _option = option || {},
        _elem, _header, _title, _tab, _content, _router,
        _opened;

    //properties
    this.option = _option;

    //methods
    this.render = function () {
        _init();
        return _elem;
    }

    this.show = function (data) {
        _opened = true;
        data = data || { Id: '' };

        if (_option.showTitle) {
            var title = _option.title;
            if (!title) {
                var tab = getCurTab();
                title = tab ? tab.title : '';
            }

            _title.html('');
            var icon = option.icon || 'fa fa-window-maximize';
            _title.append('<i>').addClass(icon).append(title);

            var actionName = data.Id === '' ? '【' + Language.New + '】' : '【' + Language.Edit + '】';
            var titleInfo = '';
            if (_option.titleInfo) {
                titleInfo = _option.titleInfo(data);
            }
            if (titleInfo.indexOf('【') > -1) {
                actionName = '';
            }
            _title.append(actionName + titleInfo);
        }

        _elem.show();
    }

    this.close = function () {
        if (_opened) {
            _elem.hide();
        }
    }

    //private
    function _init() {
        _elem = $('<div>').addClass('tabs');
        _header = $('<div>').addClass('tabs-header').appendTo(_elem);
        if (_option.showTitle)
            _title = $('<div>').addClass('tabs-title').appendTo(_header);

        _tab = $('<ul>').appendTo(_header);
        if (_option.showClose) {
            $('<i>')
                .addClass('fa fa-close close')
                .appendTo(_header)
                .click(function () {
                    _elem.hide();
                    _option.onClose && _option.onClose();
                });
        }

        _content = $('<div>').addClass('tabs-body').appendTo(_elem);

        if (_option.fit)
            _elem.addClass('tabs-fit');
        if (_option.style)
            _elem.addClass(_option.style);

        if (_option.singleTab)
            _router = new Router(_content, {});

        if (_option.items)
            _setData(_option.items);
    }

    function _setData(data) {
        for (var i = 0; i < data.length; i++) {
            var d = data[i];
            d.index = i;
            var li = $('<li>').data('item', d)
                .appendTo(_tab)
                .on('click', function () {
                    var item = $(this).data('item');
                    _itemClick($(this), item);
                });

            if (d.icon)
                $('<i>').addClass(d.icon).appendTo(li);
            li.append(d.name);

            if (!_option.singleTab && _option.renderAllTab) {
                _createTabContent(d);
            }
        }

        setTimeout(function () {
            _itemClick(_tab.find('li:eq(0)'), data[0]);
        }, 100);
    }

    function _itemClick(elem, item) {
        _tab.find('li').removeClass('active');
        elem.addClass('active');
        if (_option.singleTab) {
            _router.route(item);
        } else {
            var itemEl = _createTabContent(item);
            $('.tabs-item', _content).removeClass('active');
            itemEl.addClass('active');
        }
        _option.onItemClick && _option.onItemClick(item);
    }

    function _createTabContent(item) {
        var index = item.index || 0;
        var id = 'tab-' + index;
        var itemEl = $('#' + id);
        if (!itemEl.length) {
            itemEl = $('<div>').attr('id', id).addClass('tabs-item').appendTo(_content);
            new Router(itemEl, {}).route(item);
        }
        return itemEl;
    }
}