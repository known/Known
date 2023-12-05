window.KEditor = {
    init: function (id, option) {
        var editor = new window.wangEditor('#' + id);
        Object.assign(editor.config, option);
        editor.config.onchange = function (html) {
            KAdmin.invokeDotNet(id, 'rich.onChange', { html: html });
        };
        KEditor.customUpload(editor, id, option);
        editor.create();
        KForm.setFormList();
        return editor;
    },
    customUpload: function (editor, id, option) {
        if (option.uploadImgShowBase64)
            return;
        if (!option.storage)
            return;

        var storage = option.storage;
        if (storage.type === 0) {
            editor.config.customUploadImg = function (resultFiles, insertImgFn) {
                var file = resultFiles[0];
                var reader = new FileReader();
                reader.readAsArrayBuffer(file);
                reader.onload = function () {
                    var data = new Uint8Array(this.result);
                    var param = { name: file.name, type: file.type, data: JSON.stringify(data) };
                    KAdmin.invokeDotNet(id, 'rich.onUploadImage', param)
                        .then(res => insertImgFn(res.data))
                        .catch(err => console.log(err));
                };
            }
            editor.config.customUploadVideo = function (resultFiles, insertVideoFn) {
                var file = resultFiles[0];
                var reader = new FileReader();
                reader.readAsArrayBuffer(file);
                reader.onload = function () {
                    var data = new Uint8Array(this.result);
                    var param = { name: file.name, type: file.type, data: JSON.stringify(data) };
                    KAdmin.invokeDotNet(id, 'rich.onUploadVideo', param)
                        .then(res => insertVideoFn(res.data))
                        .catch(err => console.log(err));
                };
            }
        } else if (storage.type === 1) {
            let client = new OSS({
                region: storage.region,
                accessKeyId: storage.keyId,
                accessKeySecret: storage.keySecret,
                bucket: storage.bucket
            });
            editor.config.customUploadImg = function (resultFiles, insertImgFn) {
                client.put('myImg', resultFiles[0])
                    .then(res => insertImgFn(res.url))
                    .catch(err => console.log(err));
            }
            editor.config.customUploadVideo = function (resultFiles, insertVideoFn) {
                client.put('myVideo', resultFiles[0])
                    .then(res => insertVideoFn(res.url))
                    .catch(err => console.log(err));
            }
        } else if (storage.type === 2) {
            let cos = new COS({
                SecretId: storage.keyId,
                SecretKey: storage.keySecret
            });
            editor.config.customUploadImg = function (resultFiles, insertImgFn) {
                const file = resultFiles[0];
                cos.sliceUploadFile({
                    Bucket: storage.bucket,
                    Region: storage.region,
                    Key: file.name,
                    Body: resultFiles[0],
                }, function (err, data) {
                    if (err) {
                        console.log(err);
                        return;
                    }
                    insertImgFn('//' + data.Location);
                });
            };
            editor.config.customUploadVideo = function (resultFiles, insertVideoFn) {
                const file = resultFiles[0];
                cos.sliceUploadFile({
                    Bucket: storage.bucket,
                    Region: storage.region,
                    Key: file.name,
                    Body: resultFiles[0],
                }, function (err, data) {
                    if (err) {
                        console.log(err);
                        return;
                    }
                    insertVideoFn('//' + data.Location);
                });
            };
        }
    }
};