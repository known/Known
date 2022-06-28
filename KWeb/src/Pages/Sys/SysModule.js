/* -------------------------------------------------------------------------------
 * Copyright (c) Suzhou Puman Technology Co., Ltd. All rights reserved.
 * 
 * WebSite: https://www.pumantech.com
 * Contact: knownchen@163.com
 * 
 * Change Logs:
 * Date           Author       Notes
 * 2022-06-28     KnownChen    初始化
 * ------------------------------------------------------------------------------- */

function SysModule() {
    //methods
    this.render = function (dom) {
        var grid = EUITreeGrid('SysModule', dom);
        $(grid).treegrid({
            idField: 'id',
            treeField: 'name',
            columns: [[
                { title: '图标', field: 'icon', width: 180 },
                { title: '代码', field: 'code', width: 180 },
                { title: '名称', field: 'name', width: 180 },
                { field: '类型', title: 'type', width: 60, align: 'center' }
            ]]
        });

        $.get('/System/GetModuleTree', function (res) {
            log(res)
            var data = Utils.list2Tree(res, appId);
            $(grid).treegrid('loadData', data);
        });
    }

    //private
}

$.extend(Page, {
    SysModule: { component: new SysModule() }
});