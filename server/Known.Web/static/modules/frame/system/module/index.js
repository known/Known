var ModuleView = {

    tree: null,

    show: function () {
        $.extend(true, this, GridBase);

        var _this = this;
        var option = {
            moduleCode: 'Module',
            moduleName: '模块管理',
            form: ModuleForm,
            formView: 'System/Module/ModuleForm',
            formData: {
                Id: '',
                Enabled: 'Y'
            },
            callback: function () {
                _this.tree.reload();
            }
        };

        this.init(option);

        this.tree = mini.get('leftTree');
        this.tree.on('nodeselect', this.onTreeNodeSelect);
        this.tree.on('drop', this.onTreeDrop);
        this.showGrid('0');
    },

    showGrid: function (pid) {
        var _this = this;
        this.option.formData.ParentId = pid;
        this.grid.query.pid.setValue(pid);
        this.grid.load(function (e) {
            _this.option.formData.Sort = e.result.total + 1;
        });
    },

    onTreeNodeSelect: function (e) {
        ModuleView.showGrid(e.node.id);
    },

    onTreeDrop: function (e) {
        if (e.dragAction !== 'add')
            return;

        Ajax.postJson('/api/plt/Module/DropModule', {
            id: e.dragNode.id, pid: e.dragNode.pid
        }, function (res) {
            Message.result(res);
        });
    },

    //toolbar
    copy: function () {
        var _this = this;
        this.grid.checkSelect(function (row) {
            var data = mini.clone(row);
            data.Id = '';
            data.Sort = _this.grid.getData().length + 1;
            _this.showForm(data);
        });
    }

};

$(function () {
    mini.parse();
    ModuleView.show();
});