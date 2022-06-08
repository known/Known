function PageEditor(option) {
    var toolbar = new Toolbar({
        buttons: [
            { text: '模板', icon: 'fa fa-columns', handler: function (e) { } },
            { text: '保存', icon: 'fa fa-save', handler: function (e) { } },
            { text: '代码', icon: 'fa fa-code', handler: function (e) { } }
        ]
    });

    this.render = function (dom) {
        var elem = $('<div>').addClass('page-editor').appendTo(dom);
        toolbar.render().appendTo(elem);
        _createPagePanel(elem);
        _createPreviewPanel(elem);
        _createFieldPanel(elem);
    }

    this.load = function (node) {
        
    }

    //private
    function _createPagePanel(dom) {

    }

    function _createPreviewPanel(dom) {

    }

    function _createFieldPanel(dom) {

    }
}