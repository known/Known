var DemoForm = {
    show: function () {
    },
    getForm: function () {
        var s = Form.getData('form1', true);
        alert(s);
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
        Form.setData('form1', obj);
        Form.bindEnterJump(form);
    },
    resetForm: function () {
        Form.reset('form1');
    },
    clearForm: function () {
        Form.clear('form1');
    },
    submitForm: function () {
        //提交表单数据
        var json = Form.getData('form1', true);   //序列化成JSON
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
        $.ajax({
            url: "../data/FormService.aspx?method=LoadData",
            type: "post",
            success: function (text) {
                var data = mini.decode(text);   //反序列化成对象
                Form.setData('form1', data);    //设置多个控件数据
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