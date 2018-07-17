mini.Pager.prototype.sizeList = [10, 20, 50, 100, 200, 500, 1000, 1500, 2000];
mini.DataTable.prototype.pageSize = 20;
mini.DataGrid.prototype.allowAlternating = true;
mini.DataGrid.prototype.showColumnsMenu = true;
mini.DataGrid.prototype.showEmptyText = true;
mini.DataGrid.prototype.emptyText = '未查到任何数据！';

if (!window.UserControl) {
    window.UserControl = {};
}

/////////////////////////////////////////////////////////////////////
UserControl.IntegerBox = function () {
    UserControl.IntegerBox.superclass.constructor.call(this);
    this.bindEvents();
};

mini.extend(UserControl.IntegerBox, mini.TextBox, {
    uiCls: 'mini-integerbox',
    bindEvents: function () {
        var that = this;

        $(this.getEl()).bind('input propertychange', function () {
            var input = that.getInputText();
            that.setValue(input);
            that.setValue(input.replace(/\D/g, ''));
        });

        var oldValue;

        this.on('focus', function () {
            oldValue = that.getValue();
        });

        this.on('blur', function () {
            if (oldValue !== that.getValue()) {
                oldValue = that.getValue();
                that.doValueChanged();
            }
        });
    }
});

mini.regClass(UserControl.IntegerBox, 'integerbox');

/////////////////////////////////////////////////////////////////////
UserControl.DecimalBox = function () {
    UserControl.DecimalBox.superclass.constructor.call(this);
    this.bindEvents();
};

mini.extend(UserControl.DecimalBox, mini.TextBox, {
    uiCls: 'mini-decimalbox',
    bindEvents: function () {
        var that = this;

        $(this.getEl()).bind('input propertychange', function () {
            var input = that.getInputText();
            that.setValue(input);
            input = input.replace(/[^\d.]*/g, '');
            if (input.indexOf('.') === 0)
                input = '';
            var array = input.split('.');
            if (array.length - 1 > 1)
                input = array[0] + '.' + array[1];
            if (input.indexOf('0') === 0 && input.indexOf('.') !== 1)
                input = '0';
            that.setValue(input);
        });

        var oldValue;

        this.on('focus', function () {
            oldValue = that.getValue();
        });

        this.on('blur', function () {
            var value = this.getValue();

            if (value !== '') {
                var len = value.length;

                if (value.substring(len - 1) === '.')
                    this.setValue(value.substring(0, len - 1));
            }

            if (oldValue !== that.getValue()) {
                oldValue = that.getValue();
                that.doValueChanged();
            }
        });
    }
});

mini.regClass(UserControl.DecimalBox, 'decimalbox');

/////////////////////////////////////////////////////////////////////
mini.VTypes["plusErrorText"] = "必须大于0";
mini.VTypes["plus"] = function (v) {
    if (v != null && v != "")
        return v > 0;
    return true;
}

mini.VTypes["non-negativeErrorText"] = "必须大于等于0";
mini.VTypes["non-negative"] = function (v) {
    if (v != null && v != "")
        return v >= 0;
    return true;
}

mini.VTypes["non-zeroErrorText"] = "不能为0";
mini.VTypes["non-zero"] = function (v) {
    if (v != null && v != "")
        return v != 0;
    return true;
}

mini.VTypes["percentErrorText"] = "必须大于等于0，并且要小于100";
mini.VTypes["percent"] = function (v) {
    if (v != null && v != "")
        return v >= 0 && v < 100;
    return true;
}