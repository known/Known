function NoticeList(id) {
    $('#' + id).addClass('grid').html('<table id="gridNotice">');
    var url = {
        GetNewestNotices: baseUrl + '/System/GetNewestNotices'
    };

    var grid = new Grid('Notice', {
        url: url.GetNewestNotices, page: false, fixed: false,
        showCheckBox: false, autoQuery: false,
        columns: [
            {
                title: Language.Title, field: 'Title', format: function (d) {
                    return $('<span>')
                        .addClass('link')
                        .append(d.Title)
                        .data('data', d)
                        .click(function () {
                            var data = $(this).data('data');
                            Layer.open({
                                title: data.Title, width: 960, height: 400,
                                content: data.Content
                            });
                        });
                }
            },
            { title: Language.PublishTime, field: 'PublishTime', width: '120px', placeholder: 'yyyy-MM-dd HH:mm' }
        ]
    });

    this.load = function () {
        grid.reload();
    }

    this.more = function () {
        Layer.page({ url: baseUrl + '/?m=SysNotice&type=more' });
    }

}