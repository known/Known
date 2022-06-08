function SysNoRule() {
    var url = {
        GetNoRules: baseUrl + '/System/GetNoRules',
        DeleteModels: baseUrl + '/System/DeleteNoRules',
        SaveModel: baseUrl + '/System/SaveNoRule'
    };
    var rule;
    var list = new ListBox('listCategory', {
        url: url.GetNoRules, autoLoad: false,
        onClick: function (item) {
            rule = item;
            var rules = [];
            if (item.RuleData) {
                rules = JSON.parse(item.RuleData);
            }
            view.setGridData(rules);
        }
    });

    var ruleTips = '<div style="color:#00f;">' + Language.NoRuleTips + '</span>';
    var view = new View('SysNoRule', {
        url: url,
        left: list,
        columns: [
            { action: 'add', icon: 'fa fa-plus', align: 'center', width: '80px', aFormat: 'remove,up,down' },
            {
                title: Language.Type, field: 'Type', type: 'select', width: '100px',
                code: [Language.FixedValue, Language.DateValue, Language.SeqNo]
            },
            { title: Language.Value, field: 'Value', type: 'text' }
        ],
        gridOption: {
            edit: true, fixed: true, isTradition: true,
            toolbar: {
                add: function (e) { _addNoRule(); },
                save: function (e) { _saveNoRule(); },
                remove: function (e) { _removeNoRule(); }
            },
            footer: ruleTips
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        _reload();
    }

    //private
    function _reload() {
        view.load();
        view.setGridData([]);
    }

    function _addNoRule() {
        var form = new Form('NoRule', {
            style: 'form form-block',
            fields: [
                { field: 'Id', type: 'hidden' },
                { title: Language.System, field: 'AppId', type: 'select', code: 'ProductData', required: true },
                { title: Language.Number, field: 'Code', type: 'text', required: true },
                { title: Language.Name, field: 'Name', type: 'text', required: true },
                { title: Language.Description, field: 'Description', type: 'textarea' }
            ]
        });
        Layer.open({
            title: Language.NoRule,
            width: 450, height: 320,
            component: form,
            buttons: [{
                text: Language.OK, handler: function (e) {
                    form.save(url.SaveModel, function (data) {
                        _reload();
                        e.close();
                    });
                }
            }]
        });
    }

    function _saveNoRule() {
        if (!rule) {
            Layer.tips(Language.PleaseSelectRuleName);
            return;
        }

        var rules = view.getGridData();
        rule.RuleData = JSON.stringify(rules);
        Ajax.post(url.SaveModel, { data: JSON.stringify(rule) });
    }

    function _removeNoRule() {
        Layer.confirm(Language.ConfirmDelete, function () {
            Ajax.post(url.DeleteModels, {
                data: JSON.stringify([list.selected.Id])
            }, function () {
                _reload();
            });
        });
    }
}

$.extend(Page, {
    SysNoRule: { component: new SysNoRule() }
});