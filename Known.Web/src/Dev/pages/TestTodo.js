/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2020-08-20     KnownChen
 * ------------------------------------------------------------------------------- */

function TestTodo() {
    var url = {
        QueryModels: sysBaseUrl + '/Test/QueryTodos',
        DeleteModels: sysBaseUrl + '/Test/DeleteTodos',
        SaveModel: sysBaseUrl + '/Test/SaveTodo'
    };
    var view = new View('Todo', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            { title: '编码', field: 'Code', type: 'text', required: true },
            { title: '名称', field: 'Name', type: 'text', required: true, query: true },
            { title: '描述', field: 'Description', type: 'textarea' },
        ],
        formOption: {
            style: 'form-block'
        }
    });

    //methods
    this.render = function (dom) {
        view.render().appendTo(dom);
    }

    this.mounted = function () {
        view.load();
    }
}

$.extend(Page, {
    TestTodo: { component: new TestTodo() }
});