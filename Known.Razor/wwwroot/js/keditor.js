window.KEditor = {
    init: function (id, option) {
        var editor = new window.wangEditor('#' + id);
        Object.assign(editor.config, option);
        editor.config.onchange = function (html) {
            DotNet.invokeMethodAsync('Known.Razor', 'CallbackByParamAsync', id, 'rich.onchange', { html: html });
        };
        KEditor.customUpload(editor, option);
        editor.create();
        return editor;
    },
    customUpload: function (editor, option) {
        editor.config.customUploadImg = function (resultFiles, insertImgFn) {
            console.log(resultFiles);
            var imgUrl = '';
            insertImgFn(imgUrl);
        }
    }
};