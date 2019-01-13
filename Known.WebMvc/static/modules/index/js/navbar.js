var Navbar = {

    devTool: function () {
        var tab = MainTabs.active({
            id: 'devTool', iconCls: 'fa-puzzle-piece',
            text: '开发工具', url: '/frame?id=devTool'
        });
    },

    todo: function () {
        MainTabs.active({
            id: 'todo', iconCls: 'fa-paper-plane',
            text: '代办事项', url: '/frame?id=todo'
        });
    },

    cache: function () {
        Ajax.getJson('/api/plt/User/GetCodes', function (data) {
            Code.setData(data);
            Message.tips({ content: '刷新成功！' });
        });
    },

    info: function () {
        Ajax.getJson('/api/plt/User/GetUserInfo', function (data) {
        });
    },

    updPwd: function () {
    },

    logout: function () {
        Message.confirm('确定要退出系统？', function () {
            Ajax.postText('/signout', function () {
                location = location;
            });
        });
    }

};