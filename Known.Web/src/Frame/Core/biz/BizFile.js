const BizFileDeleteUrl = baseUrl + '/System/DeleteFile';
const BizFileDownloadUrl = baseUrl + '/System/DownloadFile';

function BizFile(type, tips) {
    //fields
    var url = {
        QueryModels: baseUrl + '/System/GetFiles',
        DeleteModels: baseUrl + '/System/DeleteFiles',
        UploadFiles: baseUrl + '/System/UploadFiles'
    };
    var _head;

    var grid = new Grid('BizFile', {
        url: url.QueryModels, querys: [], page: false, autoQuery: false, isTradition: true,
        columns: [
            { title: Language.AttachmentType, field: 'Type' },
            {
                title: Language.AttachmentName, field: 'Name', format: function (d) {
                    var icon = Utils.getExtIcon(d.ExtName);
                    var html = $('<span>').append('<i class="' + icon + '">');
                    $('<span>')
                        .addClass('link')
                        .html(d.Name)
                        .data('id', d.Id)
                        .appendTo(html)
                        .on('click', function () {
                            var id = $(this).data('id');
                            Ajax.download(BizFileDownloadUrl, { id: id });
                        });
                    return html;
                }
            }
        ],
        toolButtons: ['add', 'remove'],
        toolbarTips: (tips || ''),
        toolbar: {
            add: function (e) {
                if (!_head.Id) {
                    Layer.tips(Language.PleaseSaveBizForm);
                    return;
                }
                _addFile();
            },
            remove: function (e) {
                e.deleteRows(url.DeleteModels, function () {
                    grid.reload({ bizId: _head.Id });
                });
            }
        }
    });

    //methods
    this.render = function () {
        return grid.render();
    }

    this.load = function (e) {
        _head = e.head;
        grid.setDetail(e.isDetail);
        grid.reload({ bizId: _head.Id });
    }

    //private
    function _addFile() {
        var form;
        BizForm.open({
            title: Language.AddAttachment, width: 450, height: 200,
            fields: [
                { field: 'Category1', type: 'hidden' },
                { field: 'Category2', type: 'hidden' },
                { field: 'BizId', type: 'hidden' },
                { title: Language.AttachmentType, field: 'Type', type: 'text', required: true },
                { title: Language.Attachment, field: 'Files', type: 'file', multiple: true, required: true }
            ],
            success: function (e) {
                form = e.form;
                form.setData({ Category1: Language.FormAttachment, Category2: type, BizId: _head.Id });
            },
            buttons: [{
                text: Language.OK, handler: function (e) {
                    if (!form.validate())
                        return;

                    var data = form.getData();
                    Ajax.upload('Files', url.UploadFiles, data, function () {
                        e.close();
                        grid.reload({ bizId: _head.Id });
                    });
                }
            }]
        });
    }
}

BizFile.download = function (id) {
    Ajax.download(BizFileDownloadUrl, { id: id });
}
BizFile.delete = function (id) {
    Layer.confirm(Language.ConfirmDeleteFile, function () {
        Ajax.post(BizFileDeleteUrl, { id: id });
    });
}