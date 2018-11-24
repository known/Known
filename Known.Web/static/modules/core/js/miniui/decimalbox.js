/////////////////////////////////////////////////////////////////////
mini.DecimalBox = function () {
    mini.DecimalBox.superclass.constructor.call(this);
    this.bindEvents();
};

mini.extend(mini.DecimalBox, mini.TextBox, {
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

mini.regClass(mini.DecimalBox, 'decimalbox');