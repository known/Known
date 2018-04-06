layui.use('form', function () {
    var form = layui.form;

    form.on("submit(login)", function (data) {
        window.location.href = '/';
        return false;
    });
});