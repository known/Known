function DevDocView() {
    //fields
    var docs = [
        { id: 'ui', pid: '0', name: '前端文档', open: true },
        { id: 'uiLayout', pid: 'ui', name: '布局' },
        { id: 'uiLayer', pid: 'uiLayout', name: '树(Tree)', url: '/ui/Tree.html' },
        { id: 'uiLayer', pid: 'uiLayout', name: '视图(View)', url: '/ui/View.html' },
        { id: 'uiGrid', pid: 'uiLayout', name: '网格(Grid)', url: '/ui/Grid.html' },
        { id: 'uiForm', pid: 'uiLayout', name: '表单(Form)', url: '/ui/Form.html' },
        { id: 'uiField', pid: 'uiLayout', name: '字段(Field)', url: '/ui/Field.html' },
        { id: 'uiControl', pid: 'ui', name: '控件' },
        { id: 'uiInput', pid: 'uiControl', name: '通用控件(Input)', url: '/ui/control/Input.html' },
        { id: 'uiCheckBox', pid: 'uiControl', name: '复选框(CheckBox)', url: '/ui/control/CheckBox.html' },
        { id: 'uiCheckBoxList', pid: 'uiControl', name: '复选框列表(CheckBoxList)', url: '/ui/control/CheckBoxList.html' },
        { id: 'uiRadioBoxList', pid: 'uiControl', name: '单选框列表(RadioBoxList)', url: '/ui/control/RadioBoxList.html' },
        { id: 'uiSelectList', pid: 'uiControl', name: '下拉列表(SelectList)', url: '/ui/control/SelectList.html' },
        { id: 'uiListBox', pid: 'uiControl', name: '列表框(ListBox)', url: '/ui/control/ListBox.html' },
        { id: 'uiLayer', pid: 'ui', name: '弹层(Layer)', url: '/ui/Layer.html' },
        { id: 'uiFrame', pid: 'ui', name: '框架(Frame)', url: '/ui/Frame.html' },
        { id: 'back', pid: '0', name: '后端文档', open: true },
        { id: 'backKnown', pid: 'back', name: 'Known', url: '/back/known.html' },
        { id: 'backKnownNet', pid: 'back', name: 'Known.Net', url: '/back/knownnet.html' }
    ];
    var content;
    var tree = new Tree('tree', {
        data: docs,
        onClick: function (node) {
            if (!node.children) {
                content.html('');
                content.load(baseUrl + node.url, function () {
                    var pre = $('pre.code').addClass('prettyprint source linenums');
                    var code = pre.html();
                    pre.html('<code>' + code + '</code>');
                    prettyPrint();
                });
            }
        }
    });

    //methods
    this.render = function (dom) {
        Utils.addJs('/libs/prettify/prettify.js');
        Utils.addJs('/libs/prettify/lang-css.js');

        var fit = $('<div>').addClass('fit doc').appendTo(dom);
        var left = $('<div>').addClass('fit-col-3').appendTo(fit);
        tree.render().appendTo(left);
        content = $('<div>').addClass('fit-col-7 content').appendTo(fit);
    }

    this.mounted = function () {
    }
}

$.extend(Page, {
    DevDocView: { component: new DevDocView() }
});