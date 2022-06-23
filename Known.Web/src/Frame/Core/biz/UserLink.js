/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * 2022-06-23     KnownChen    常用连接地址https问题
 * ------------------------------------------------------------------------------- */

function UserLink(id, type) {
    //field
    var url = {
        GetUserLinks: baseUrl + '/System/GetUserLinks',
        DeleteUserLink: baseUrl + '/System/DeleteUserLink',
        AddUserLink: baseUrl + '/System/AddUserLink',
        AddShortcuts: baseUrl + '/System/AddShortcuts'
    };
    var menus = top.Admin.Menus;

    var links = {
        shortcut: {
            getDom: function (data) {
                var dataName = 'data';
                var item = menus.filter(function (d) { return d.id === data.Address; });
                var menu = item.length ? item[0] : {};
                var li = $('<li>').data(dataName, menu).click(function () {
                    var d = $(this).data(dataName);
                    showTabPage(d);
                });
                li.append('<span class="icon"><span class="' + menu.icon + '"></span></span>');
                li.append('<span>' + menu.name + '</span>');
                return li;
            },
            add: function () {
                var treeLink;
                Layer.open({
                    title: Language.Shortcut, icon: 'fa fa-external-link',
                    width: 360, height: 300,
                    content: '<ul id="treeLink"></ul>',
                    success: function () {
                        for (var i = 0; i < menus.length; i++) {
                            if (itemAddresses.indexOf(menus[i].id) > -1) {
                                menus[i].checked = true;
                            }
                        }
                        treeLink = new Tree('treeLink', { data: menus, check: true });
                    },
                    buttons: [{
                        text: Language.OK, handler: function (e) {
                            var nodes = treeLink.getCheckedNodes();
                            if (nodes.length === 0) {
                                Layer.tips(Language.PleaseSelectMenuItem);
                            } else {
                                var data = [], address = [];
                                for (var i = 0; i < nodes.length; i++) {
                                    var node = nodes[i];
                                    if (!node.children) {
                                        if (address.indexOf(node.url) < 0) {
                                            data.push({ Type: type, Name: node.name, Address: node.id });
                                            address.push(node.id);
                                        }
                                    }
                                }
                                $.post(url.AddShortcuts, { data: JSON.stringify(data) }, function () {
                                    e.close();
                                    _load();
                                });
                            }
                        }
                    }]
                });
            }
        },
        commlink: {
            getDom: function (data) {
                var dataName = 'data';
                var li = $('<li>').data(dataName, data).click(function () {
                    var d = $(this).data(dataName);
                    window.open(d.Address);
                });
                li.append('<span>' + data.Name + '</span>');
                return li;
            },
            add: function () {
                var _this = this, form;
                Layer.open({
                    title: Language.CommonLink, icon: 'fa fa-external-link',
                    width: 400, height: 220,
                    content: '<div id ="formLink" class="form form-block">',
                    success: function () {
                        form = new Form('Link', {
                            labelWidth: 60,
                            fields: [
                                { title: Language.Name, field: 'Name', type: 'text', required: true },
                                { title: Language.Address, field: 'Address', type: 'text', required: true, inputStyle: 'width:280px' }
                            ],
                            onSaving: function (d) {
                                d.Type = type;
                                if (d.Address.indexOf('http') < 0) {
                                    d.Address = 'http://' + d.Address;
                                }
                            }
                        });
                    },
                    buttons: [{
                        text: Language.OK, handler: function (e) {
                            form.save(url.AddUserLink, function (data) {
                                e.close();
                                _createItem(data);
                            });
                        }
                    }]
                });
            }
        }
    };
    var elem = $('#' + id).addClass(type);
    var itemAddresses = [], link = links[type];

    //method
    this.load = function () {
        _load();
    }

    this.add = function () {
        link.add();
    }

    //private
    function _load() {
        elem.html(Language.Loading + '......');
        $.get(url.GetUserLinks, { type: type }, function (res) {
            elem.html('');
            for (var i = 0; i < res.length; i++) {
                _createItem(res[i]);
            }
        });
    }

    function _createItem(data) {
        itemAddresses.push(data.Address);
        var li = link.getDom(data);
        li.hover(function () {
            var dataName = 'data';
            $('<span class="close fa fa-close">')
                .data(dataName, data)
                .click(function (e) {
                    var btn = $(this), parent = btn.parent(), d = btn.data(dataName);
                    Layer.confirm(Language.ConfirmDelete, function () {
                        $.post(url.DeleteUserLink, { id: d.Id }, function () {
                            parent.remove();
                        });
                    });
                    e.stopPropagation();
                }).appendTo($(this));
        }, function () {
            $(this).find('.close').remove();
        }).appendTo(elem);
    }

}