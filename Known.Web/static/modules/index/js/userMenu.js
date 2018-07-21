var UserMenu = {

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