layui.use(['form', 'layer'], function () {
    var form = layui.form,
        layer = parent.layer === undefined ? layui.layer : parent.layer,
        $ = layui.jquery;

    form.on("submit(login)", function (data) {
        //window.location.href = "../../index.html";
        layer.msg(data);
        return false;
    })
});
