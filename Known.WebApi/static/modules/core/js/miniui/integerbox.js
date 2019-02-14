/////////////////////////////////////////////////////////////////////
mini.IntegerBox = function () {
    mini.IntegerBox.superclass.constructor.call(this);
    this.bindEvents();
};

mini.extend(mini.IntegerBox, mini.TextBox, {
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

mini.regClass(mini.IntegerBox, 'integerbox');