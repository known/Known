var DemoForm = {

    form: null,

    show: function () {
        var toolbar = new Toolbar('tbDemo', this);
        //toolbar.setLabel.click();

        this.form = new Form('formDemo');
    },

    getForm: function () {
        var s = this.form.getData(true);
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
        this.form.setData(obj);
    },

    resetForm: function () {
        this.form.reset();
    },

    clearForm: function () {
        this.form.clear();
    },

    submitForm: function () {
        //提交表单数据
        var json = this.form.getData(true);   //序列化成JSON
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
        var _this = this;
        //加载表单数据
        $.ajax({
            url: "../data/FormService.aspx?method=LoadData",
            type: "post",
            success: function (text) {
                var data = mini.decode(text);   //反序列化成对象
                _this.form.setData(data);    //设置多个控件数据
            }
        });
    },

    setLabel: function () {
        this.form.model(true);
    },

    setInput: function () {
        this.form.model(false);
    }

};