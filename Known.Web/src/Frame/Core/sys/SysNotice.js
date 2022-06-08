function SysNotice() {
    var type = Utils.getUrlParam('type'),
        url = {
            QueryModels: baseUrl + '/System/QueryNotices',
            DeleteModels: baseUrl + '/System/DeleteNotices',
            SaveModel: baseUrl + '/System/SaveNotice',
            PublishNotice: baseUrl + '/System/PublishNotice'
        };

    var view = new View('SysNotice', {
        url: url,
        columns: [
            { field: 'Id', type: 'hidden' },
            {
                title: Language.Title, field: 'Title', query: true, format: function (d) {
                    return $('<span class="link">')
                        .append(d.Title)
                        .data('data', d)
                        .click(function () {
                            var data = $(this).data('data');
                            Layer.open({
                                title: data.Title, width: 960, height: 500,
                                content: data.Content
                            });
                        });
                },
                type: 'text', required: true, lineBlock: true, inputStyle: 'width:50%'
            },
            { onlyForm: true, title: Language.Content, field: 'Content', type: 'editor', required: true, lineBlock: true },
            { title: Language.Status, field: 'Status' },
            { title: Language.PublishBy, field: 'PublishBy', type: 'text' },
            { title: Language.PublishTime, field: 'PublishTime', placeholder: 'yyyy-MM-dd HH:mm', type: 'date' }
        ],
        formToolbar: [{
            text: Language.Publish, icon: 'fa fa-send-o', handler: function (e) {
                e.form.save(url.PublishNotice, function () {
                    view.loadGrid();
                });
            }
        }],
        gridOption: {
            where: { type: type }
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
    SysNotice: { component: new SysNotice() }
});