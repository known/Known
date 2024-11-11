function K_TableAutoFill() {
    if ($('.ant-modal-body .ant-table-body').length)
        return;

    var parent = $('.kui-nav-tabs').length ? '.ant-tabs-tabpane-active' : '.kui-body';
    var table = $(parent + ' .ant-table-body');
    if (table.data('autofill'))
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
    }
};