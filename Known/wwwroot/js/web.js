function K_AutoFillHeight(isResize) {
    if ($('.ant-modal-body .ant-table-body').length)
        return;

    var parent = $('.kui-nav-tabs').length ? '.ant-tabs-tabpane-active' : '.kui-body';
    var table = $(parent + ' .ant-table-body');
    //console.log(parent);
    if (table.data('autofill') && !isResize)
        return;

    var height = $('.kui-layout .kui-body').height();
    height -= KUtils.getHeight(parent + ' .kui-query');
    height -= KUtils.getHeight(parent + ' .kui-toolbar');
    height -= KUtils.getHeight(parent + ' .ant-tabs-nav');
    height -= KUtils.getHeight(parent + ' .ant-table-header');
    height -= KUtils.getHeight(parent + ' .ant-table-pagination', 10);
    if (height < 200)
        height = 200;
    table.height(height + 'px');
    table.data('autofill', true);
}

var KUtils = {
    getHeight: function (selector, defaultHeight) {
        var height = 0;
        var elem = $(selector);
        if (elem.length)
            height = elem.outerHeight(true);
        else
            height = defaultHeight || 0;
        //console.log(selector + '=' + height);
        return height;
    },
    highlight: function (code, lang) {
        return Prism.highlight(code, Prism.languages[lang], lang);
    }
};

$(function () {
    window.Prism = window.Prism || {};
    Prism.disableWorkerMessageHandler = true;
    Prism.manual = true;
    $(window).resize(function () {
        K_AutoFillHeight(true);
    });
});

window.isMobile = function () {
    const userAgent = navigator.userAgent || navigator.vendor || window.opera;
    return /android|iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
};