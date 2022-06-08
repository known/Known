Picker.action = {};
function Picker(elem, option) {
    //field
    var _elem = typeof elem === 'string' ? $('[name="' + elem + '"]') : elem,
        _option = option || {},
        _multiSelect = false,
        _id, _name = _option.field,
        _text,
        _dataItemName = 'data';

    //init
    _init();

    //property
    this.name = _name;

    //method
    this.validate = function () {
        var value = $.trim(this.getValue());
        if (!_elem[0].disabled && _elem[0].required && value === '') {
            return false;
        }

        return true;
    }

    this.getValue = function () {
        var d = _elem.data(_dataItemName);
        return d && d.value ? d.value : _elem.val();
    }

    this.getData = function () {
        var d = _elem.data(_dataItemName);
        return d && d.data ? d.data : null;
    }

    this.setValue = function (data) {
        if (!data)
            return;

        _elem.val(data.value);
        if (_text) {
            _text.val(data.text);
        }
    }

    //private
    function _init() {
        var pick = _option.pick || {};
        _id = _name;
        if (pick.action) {
            _id = pick.action + _id;
            pick = Picker.action[pick.action] || {};
            $.extend(true, pick, _option.pick);
        }
        _multiSelect = pick.multiSelect || false;

        var btn = $('<span>').addClass('icon fa fa-ellipsis-h').on('click', function () {
            _openPicker(pick);
        });
        if (pick.valueField === pick.textField) {
            _elem.attr('type', 'text').attr('disabled', true);
            btn.insertAfter(_elem);
        } else {
            var input = new Input(_elem.parent(), {
                field: _name + 'Name', type: 'text', readonly: true
            });
            _text = input.elem;
            if (_option.form) {
                _option.form.setInput(input);
            }
            _text.attr('disabled', true);
            btn.insertAfter(_text);
        }
    }

    function _openPicker(pick) {
        if (pick.type === 'component') {
            _openComponentPicker(pick);
        } else if (pick.type === 'tree') {
            _openTreePicker(pick);
        } else {
            _openGridPicker(pick);
        }
    }

    function _openComponentPicker(pick) {
        var dlg = Layer.open({
            title: pick.title,
            width: pick.width || 600, height: pick.height || 400,
            content: function (body) {
                pick.component.render(body);
                pick.onLoad && pick.onLoad({
                    form: _option.form, control: pick.component, option: pick
                });
            },
            buttons: [{
                text: Language.OK, handler: function (e) {
                    _componentCallback(dlg, pick);
                }
            }]
        });
    }

    function _openTreePicker(pick) {
        var obj;
        var dlg = Layer.open({
            title: pick.title,
            width: pick.width || 600, height: pick.height || 400,
            content: function (body) {
                obj = new Tree(_id + 'TreePick', { url: pick.url });
                obj.render().appendTo(body);
                pick.onLoad && pick.onLoad({ form: _option.form, control: obj, option: pick });
            },
            buttons: [{
                text: Language.OK, handler: function (e) {
                    var node = obj.selectedNode;
                    if (!node) {
                        Layer.tips(Language.PleaseSelect + Language.Node + '！');
                        return;
                    }
                    _treeCallback(dlg, pick, node);
                }
            }]
        });
    }

    function _openGridPicker(pick) {
        var obj;
        var dlg = Layer.open({
            title: pick.title,
            width: pick.width || 600, height: pick.height || 400,
            content: function (body) {
                pick.showCheckBox = true;
                pick.multiSelect = _multiSelect;
                if (!_multiSelect) {
                    pick.dblclick = function (e) {
                        _gridCallback(dlg, pick, [e.row]);
                    }
                }
                var option = $.extend({}, pick, { width: null });
                obj = new Grid(_id + 'Pick', option);
                obj.render().appendTo(body);

                if (pick.saveUrl) {
                    Utils.createButton({
                        icon: 'fa fa-plus', text: Language.Add, handler: function () {
                            _showGridForm(pick, function (data) {
                                if (_multiSelect) {
                                    obj.reload();
                                } else {
                                    _gridCallback(dlg, pick, [data]);
                                }
                            });
                        }
                    }).appendTo(obj.query.elem);
                } else if (pick.onAddClick) {
                    Utils.createButton({
                        icon: 'fa fa-plus', text: Language.Add, handler: function () {
                            pick.onAddClick({ control: obj, option: pick });
                        }
                    }).appendTo(obj.query.elem);
                }
                pick.onLoad && pick.onLoad({ form: _option.form, control: obj, option: pick });
            },
            buttons: [{
                text: Language.OK, handler: function (e) {
                    var rows = obj.getSelected();
                    if (!_multiSelect) {
                        if (!rows || rows.length === 0 || rows.length > 1) {
                            Layer.tips(Language.PleaseSelectOne);
                            return;
                        }
                    } else {
                        if (!rows || rows.length === 0) {
                            Layer.tips(Language.PleaseSelectOneAtLeast);
                            return;
                        }
                    }
                    _gridCallback(dlg, pick, rows);
                }
            }]
        });
    }

    function _showGridForm(pick, callback) {
        var form = new Form(_id + 'Pick', {
            style: 'form form-block',
            fields: pick.columns,
            data: pick.formData
        });
        var dlg = Layer.open({
            title: pick.title + '【' + Language.New + '】',
            width: pick.formWidth || 600, height: pick.formHeight || 350,
            component: form,
            buttons: [{
                text: Language.OK, handler: function (e) {
                    form.save(pick.saveUrl, function (data) {
                        callback && callback(data);
                        dlg.close();
                    });
                }
            }]
        });
    }

    function _componentCallback(dlg, pick) {
        var res = { form: _option.form };
        var data = pick.component.getData();
        res.value = data[pick.valueField];
        res.text = data[pick.textField];
        res.data = data;
        _callback(dlg, pick, res);
    }

    function _treeCallback(dlg, pick, node) {
        var res = { form: _option.form };
        res.value = node.id;
        res.text = node.title;
        res.data = node.data;
        _callback(dlg, pick, res);
    }

    function _gridCallback(dlg, pick, rows) {
        var res = { form: _option.form };
        if (_multiSelect) {
            var values = [], texts = [];
            for (var i = 0; i < rows.length; i++) {
                values.push(rows[i][pick.valueField]);
                texts.push(rows[i][pick.textField]);
            }
            res.value = values.join(',');
            res.text = texts.join(',');
            res.data = rows;
        } else {
            var row = rows[0];
            res.value = row[pick.valueField];
            res.text = row[pick.textField];
            res.data = row;
        }
        _callback(dlg, pick, res);
    }

    function _callback(dlg, pick, res) {
        _elem.data(_dataItemName, res.data);
        _elem.val(res.value);
        if (_text) {
            _text.val(res.text);
        }
        pick.callback && pick.callback(res);
        dlg.close();
    }
}