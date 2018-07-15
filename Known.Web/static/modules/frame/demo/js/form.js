var DemoForm = {
    getForm: function () {
        var form = new mini.Form("#form1");
        var data = form.getData(true, false);
        var s = mini.encode(data);
        alert(s);
        //form.setIsValid(false);
    },
    setForm: function () {
        var obj = {
            String: "abc",
            Date: "2020-11-12",
            Boolean: 'Y',
            TreeSelect: "ajax",
            countrys: "cn",
            //countrys2: "de",
            countrys3: "usa"
        };
        var form = new mini.Form("#form1");
        form.setData(obj, false);
        Form.bindEnterJump(form);
    },
    resetForm: function () {
        var form = new mini.Form("#form1");
        form.reset();
    },
    clearForm: function () {
        var form = new mini.Form("#form1");
        form.clear();
    },
    submitForm: function () {
        //提交表单数据
        var form = new mini.Form("#form1");
        var data = form.getData();      //获取表单多个控件的数据
        var json = mini.encode(data);   //序列化成JSON
        $.ajax({
            url: "../data/FormService.aspx?method=SaveData",
            type: "post",
            data: { submitData: json },
            success: function (text) {
                alert("提交成功，返回结果:" + text);
            }
        });

    },
    loadForm: function () {
        //加载表单数据
        var form = new mini.Form("#form1");
        $.ajax({
            url: "../data/FormService.aspx?method=LoadData",
            type: "post",
            success: function (text) {
                var data = mini.decode(text);   //反序列化成对象
                form.setData(data);             //设置多个控件数据
            }
        });
    },
    setLabel: function () {
        Form.model('form1', true);
    },
    setInput: function () {
        Form.model('form1', false);
    }
};