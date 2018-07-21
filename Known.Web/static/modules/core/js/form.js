///////////////////////////////////////////////////////////////////////
var Form = function (formId, options) {
    this.formId = formId;
    this.form = new mini.Form('#' + formId);
    this.options = $.extend(true, {}, this.options, options);

    var inputs = this.form.getFields();
    for (var i = 0; i < inputs.length; i++) {
        var input = inputs[i];
        this[input.id] = input;
    }
}

Form.prototype = {

    options: {
    },

    reset: function () {
        this.form.reset();
    },

    clear: function (controls) {
        if (controls) {
            var _this = this;
            $(controls.split(',')).each(function (i, c) {
                var control = mini.getbyName(c, _this.form);
                if (control) {
                    control.setValue('');
                    if (control.type === 'autocomplete') {
                        control.setText('');
                    }
                }
            });
        } else {
            this.form.clear();
        }
    },

    validate: function (tabsId, tabIndex) {
        if (this.form.validate())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }
        return false;
    },

    getData: function (encode) {
        var data = this.form.getData(true);
        return encode ? mini.encode(data) : data;
    },

    setData: function (data, callback) {
        if (data) {
            this.form.setData(data);
            callback && callback(data);
            this.form.setChanged(false);
        }
    },

    bindEnterJump: function () {
        var inputs = this.form.getFields();
        var activeIndexes = [];

        for (var i = 0, len = inputs.length; i < len; i++) {
            var input = inputs[i];
            $(input.getEl()).unbind('keyup');

            if (input.type !== 'hidden' &&
                input.type !== 'checkbox' &&
                input.type !== 'checkboxlist' &&
                input.type !== 'radiobuttonlist' &&
                input.type !== 'htmlfile' &&
                input.getEnabled() === true &&
                input.getVisible() === true)
                activeIndexes.push(i);
        }

        for (var i = 0, len = activeIndexes.length; i < len; i++) {
            (function (i) {
                var index = activeIndexes[i];
                var nextIndex = activeIndexes[i + 1];

                if (i === len - 1) {
                    nextIndex = activeIndexes[0];
                }

                var current = inputs[index];
                $(current.getEl()).keyup(function (e) {
                    if (e.keyCode === 13) {
                        var nextInput = inputs[nextIndex];
                        setTimeout(function () {
                            nextInput.focus();
                            if (nextInput.type !== 'textarea') {
                                nextInput.selectText();
                            }
                        }, 10);
                    } else if (i > 0 && e.keyCode === 38) {
                        var preInput = inputs[activeIndexes[i - 1]];
                        if (current.type !== 'textarea' && (
                            (current.type !== 'autocomplete' && current.type !== 'combobox')
                            || !current.isShowPopup()
                        )) {
                            setTimeout(function () {
                                preInput.focus();
                                if (preInput.type !== 'textarea') {
                                    preInput.selectText();
                                }
                            }, 10);
                        }
                    }
                });
            })(i);
        }
    },

    model: function (isLabel) {
        var labelClass = 'form-input-label-model';
        $('span.' + labelClass).remove();
        var inputs = this.form.getFields();
        for (var i = 0, len = inputs.length; i < len; i++) {
            var input = inputs[i];
            input.setVisible(!isLabel);

            if (input.type === 'hidden' || !isLabel)
                continue;

            var text = input.getValue();
            if (input.type === 'combobox' ||
                input.type === 'autocomplete' ||
                input.type === 'listbox' ||
                input.type === 'checkbox' ||
                input.type === 'checkboxlist' ||
                input.type === 'radiobuttonlist' ||
                input.type === 'datepicker' ||
                input.type === 'timespinner') {
                text = input.getText();
            } else if (input.type === 'textarea') {
                text = text.htmlEncode();
            }

            var html = '<span class="' + labelClass + '">' + text + '</span>';
            $(input.getEl()).after(html);
        }
    }

};