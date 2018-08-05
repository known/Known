var DevDatabase = {

    grid: null,
    toolbar: null,
    form: null,

    show: function () {
        this.grid = new Grid('Database');
        this.toolbar = new Toolbar('tbDatabase', this);
        this.form = new Form('formDatabase');
    },

    //toolbar
    new: function () {
        this.form.clear();
    },

    query: function () {
        if (!this.form.validate())
            return;

        this.grid.set({
            columns: [
                { type: "indexcolumn" },
                { field: "loginname", width: 120, headerAlign: "center", allowSort: true, header: "员工账号", editor: { type: "textbox", minValue: 0, maxValue: 200, value: 25 } },
                { field: "age", width: 100, headerAlign: "center", allowSort: true, header: "年龄", editor: { type: "spinner" } },
                { field: "birthday", width: 100, headerAlign: "center", dateFormat: "yyyy-MM-dd H:mm", allowSort: true, header: "生日", editor: { type: "datepicker" } },
                { field: "remarks", width: 120, headerAlign: "center", allowSort: true, header: "备注", editor: { type: "textarea" } },
                { field: "gender", type: "comboboxcolumn", autoShowPopup: true, width: 100, headerAlign: "center", header: "性别", editor: { type: "combobox", data: Genders } },
                { field: "country", type: "comboboxcolumn", width: 100, headerAlign: "center", header: "国家", editor: { type: "combobox", url: "../data/countrys.txt" } },
                { field: "married", trueValue: 1, falseValue: 0, type: "checkboxcolumn", width: 60, headerAlign: "center", header: "婚否" }
            ]
        });

        this.grid.load();
    }

};