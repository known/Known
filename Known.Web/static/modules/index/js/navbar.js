var Navbar = {
    demo: function () {
        var tab = MainTabs.active({
            id: 'demo', iconCls: 'fa-puzzle-piece',
            text: '开发示例', url: '/frame/demoview'
        });
    },
    todo: function () {
        MainTabs.active({
            id: 'todo', iconCls: 'fa-paper-plane',
            text: '代办事项', url: '/home/todo'
        });
    }
};