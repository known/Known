///////////////////////////////////////////////////////////////////////
var Form = {
    get: function (form) {
        if (typeof form === 'string')
            return new mini.Form('#' + form);

        return form;
    },
    reset: function (form) {
        this.get(form).reset();
    },
    clear: function (form, controls) {
        if (controls) {
            $(controls.split(',')).each(function (i, c) {
                var control = mini.getbyName(c, form);
                if (control) {
                    control.setValue('');
                    if (control.type === 'autocomplete') {
                        control.setText('');
                    }
                }
            });
        } else {
            this.get(form).clear();
        }
    },
    validate: function (form, tabsId, tabIndex) {
        if (this.get(form).validate())
            return true;

        if (tabsId) {
            var tabs = mini.get(tabsId);
            var tab = tabs.getTab(index);
            tabs.activeTab(tab);
        }
        return false;
    },
    getData: function (form, encode) {
        var data = this.get(form).getData(true);
        return encode ? mini.encode(data) : data;
    },
    setData: function (form, data, callback) {
        var ctl = this.get(form);
        if (ctl && data) {
            ctl.setData(data);
            callback && callback(data);
            ctl.setChanged(false);
        }
    },
    bindEnterJump: function (form) {
        var inputs = this.get(form).getFields();
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
    model: function (form, isLabel) {
        var labelClass = 'form-input-label-model';
        $('span.' + labelClass).remove();
        var inputs = this.get(form).getFields();
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