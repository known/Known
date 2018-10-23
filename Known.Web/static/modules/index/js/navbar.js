var Navbar = {

    devTool: function () {
        var tab = MainTabs.active({
            id: 'devTool', iconCls: 'fa-puzzle-piece',
            text: '开发工具', url: '/frame?mid=devTool'
        });
    },

    todo: function () {
        MainTabs.active({
            id: 'todo', iconCls: 'fa-paper-plane',
            text: '代办事项', url: '/frame?mid=todo'
        });
    }

};