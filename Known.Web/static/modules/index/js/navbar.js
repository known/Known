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
    },

    cache: function () {
        Message.tips({ content: '刷新成功！' });
    },

    info: function () {
        Ajax.getJson('/api/user/getuserinfo', function (data) {
        });
    },

    updPwd: function () {
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/user/signout', function () {
                location = location;
            });
        });
    }

};