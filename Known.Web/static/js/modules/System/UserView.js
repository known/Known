layui.define('index', function (exports) {
    var url = {
        GetUserInfo: '/System/GetUserInfo',
        SaveUserInfo: '/System/SaveUserInfo',
        UpdatePassword: '/System/UpdatePassword'
    };
    layui.use(['form', 'element'], function () {
        var $ = layui.jquery,
            form = layui.form,
            layer = layui.layer;

        
    });

    exports('/System/UserView', {});
});