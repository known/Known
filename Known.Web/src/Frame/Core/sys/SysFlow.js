function SysFlow(flow) {
    //fields
    var url = {
        GetModel: baseUrl + '/System/GetFlowStep',
        SaveModel: baseUrl + '/System/SaveFlowStep'
    };

    flow = flow || { Code: 'SysFlow' };

    var chart = new FlowChart(flow.Code, {
        items: flow.steps, click: function (e) {
            $.get(url.GetModel, {
                flowCode: flow.Code,
                stepCode: e.step.id
            }, function (res) {
                if (!res) {
                    res = { Id: '', FlowCode: flow.Code, FlowName: flow.Name };
                }
                res.StepCode = e.step.id;
                res.StepName = e.step.name;
                res.StepType = e.step.flag;
                res.X = e.step.x;
                res.Y = e.step.y;
                res.IsRound = e.step.round ? 1 : 0;
                form.setData(res);
            });
        }
    });

    var form = new Form(flow.Code, {
        style: 'form-block', labelWidth: 60,
        fields: [
            { field: 'Id', type: 'hidden' },
            { field: 'FlowCode', type: 'hidden' },
            { field: 'FlowName', type: 'hidden' },
            { field: 'StepCode', type: 'hidden' },
            { field: 'StepType', type: 'hidden' },
            { field: 'X', type: 'hidden' },
            { field: 'Y', type: 'hidden' },
            { field: 'IsRound', type: 'hidden' },
            { field: 'Arrows', type: 'hidden' },
            { title: Language.Name, field: 'StepName', type: 'text', readonly: true },
            { title: Language.OperateBy, field: 'OperateBy', type: 'picker', pick: { action: 'user' } },
            { title: Language.Description, field: 'Note', type: 'textarea' }
        ],
        toolbar: [{
            text: Language.OK, handler: function (e) {
                form.save(url.SaveModel);
            }
        }]
    });

    //methods
    this.render = function (dom) {
        var elem = $('<div>').addClass('fit-col').css('overflow', 'hidden').appendTo(dom);
        chart.render(elem);
        form.render().appendTo(elem);
        form.elem.addClass('form-flow').css({ width: '300px', height: '300px' });
    }

    this.mounted = function () {
    }

    //private
}

$.extend(Page, {
    SysFlow: { component: new SysFlow() }
});