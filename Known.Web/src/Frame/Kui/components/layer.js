var Layer = {

    //field
    index: 10000,

    //public
    page: function (option) {
        return this._show('page', option);
    },

    open: function (option) {
        option.showMax = option.showMax === undefined ? true : option.showMax;
        return this._show('dialog', option);
    },

    loading: function (message) {
        message = message || '';
        if (message === '')
            return { close: function () { } };

        var _this = this, index = _this.index++, maskId = 'mask' + index, loadId = 'load' + index;
        $('<div>').addClass('mask')
            .attr('id', maskId)
            .css({ zIndex: index })
            .appendTo($('body'));

        var load = $('<div>').addClass('loading')
            .attr('id', loadId)
            .append('<i class="fa fa-spinner fa-spin fa-1x fa-fw">')
            .append(message);
        var width = load.outerWidth();
        load.css({ marginLeft: -(width / 2) + 'px', zIndex: index + 1 }).appendTo($('body'));
        return {
            close: function () {
                $('#' + maskId + ',#' + loadId).remove();
                _this.index--;
            }
        };
    },

    tips: function (message) {
        message = message || '';
        if (message === '')
            return;

        var tip = $('.tips');
        if (!tip.length) {
            tip = $('<div>').addClass('tips').css({ zIndex: this.index }).appendTo($('body'));
        }
        tip.html(message);
        var width = tip.outerWidth();
        var height = tip.outerHeight();
        tip.css({ marginLeft: -(width / 2) + 'px', marginTop: -(height / 2) + 'px' });
        setTimeout(function () { tip.remove(); }, 3000);
    },

    alert: function (message, callback) {
        var _this = this;
        var height = 150 - 42 - 42 - 10;
        var dlg = _this._show('dialog', {
            showMax: false,
            icon: 'fa fa-info-circle', title: Language.Tips, width: 300, height: 150,
            content: function (dom) {
                var div = $('<div>').css({
                    textAlign: 'center', height: height + 'px'
                }).appendTo(dom);
                $('<div>').css('padding-top', '15px').html(message).appendTo(div);
            },
            footer: function (dom) {
                _this._createOK(dom, function () {
                    callback && callback();
                    dlg.close();
                });
            }
        });
    },

    confirm: function (message, callback) {
        var _this = this;
        var height = 150 - 42 - 42 - 10;
        var dlg = _this._show('dialog', {
            showMax: false,
            icon: 'fa fa-question-circle', title: Language.Confirm, width: 300, height: 150,
            content: function (dom) {
                var div = $('<div>').css({
                    textAlign: 'center', height: height + 'px'
                }).appendTo(dom);
                $('<div>').css('padding-top', '15px').html(message).appendTo(div);
            },
            footer: function (dom) {
                _this._createOK(dom, function () {
                    callback && callback();
                    dlg.close();
                });
                $('<span>').css({ width: '20px', display: 'inline-block' }).appendTo(dom);
                _this._createCancel(dom, function () {
                    dlg.close();
                });
            }
        });
    },

    changeMax: function (btn, target, css, zindex) {
        if (btn.data('isMax')) {
            btn.data('isMax', false)
                .removeClass('fa-window-restore')
                .addClass('fa-window-maximize');
            target.removeClass(css).attr('style', btn.data('layerStyle'));
        } else {
            btn.data('isMax', true)
                .data('layerStyle', target.attr('style'))
                .removeClass('fa-window-maximize')
                .addClass('fa-window-restore');
            target.addClass(css).attr('style', '').css({ zIndex: zindex });
        }
    },

    //private
    _createOK: function (dom, callback) {
        $('<button>').addClass('ok').html(Language.OK).appendTo(dom).on('click', callback);
    },

    _createCancel: function (dom, callback) {
        $('<button>').addClass('cancel').html(Language.Cancel).appendTo(dom).on('click', callback);
    },

    _show: function (type, option) {
        var _this = this, index = _this.index++,
            maskId = 'mask' + index,
            layerId = 'layer' + index;
        $('<div>').attr('id', maskId)
            .addClass('mask')
            .css({ zIndex: index })
            .appendTo($('body'));
        var layer = $('<div>')
            .attr('id', layerId)
            .addClass(type)
            .css({ zIndex: index + 1 })
            .appendTo($('body'));
        var dlg = {
            index: index, layer: layer, close: function () {
                $('#' + maskId + ',#' + layerId).remove();
                _this.index--;
            }
        };

        function createMaximizeIcon(container) {
            return $('<i>')
                .addClass('fa fa-window-maximize maximize')
                .data('isMax', false)
                .appendTo(container)
                .click(function () {
                    _this.changeMax($(this), layer, 'dialog-max', index + 1);
                });
        }

        function createCloseIcon(container) {
            $('<i>')
                .addClass('fa fa-close close')
                .appendTo(container)
                .click(close);
        }

        function close() {
            dlg.close();
            option.onClose && option.onClose();
        }

        var div = {};
        function setHeadMouseEvent(header) {
            header.mousedown(function (e) {
                e.preventDefault();
                div.move = true;
                div.offset = [
                    e.clientX - parseFloat(layer.css('left')),
                    e.clientY - parseFloat(layer.css('top'))
                ];
            });
            $(document).mousemove(function (e) {
                e.preventDefault();
                if (div.move) {
                    var left = e.clientX - div.offset[0];
                    var top = e.clientY - div.offset[1];
                    layer.css({ left: left, top: top });
                }
            }).mouseup(function () {
                delete div.move;
            });
        }

        function createContent(body, page) {
            if (option.content) {
                if (page) {
                    body.css({ overflow: 'auto' });
                }
                parseDom(body, option.content);
            } else if (option.url) {
                if (option.partial) {
                    if (page) {
                        body.css({ overflow: 'auto' });
                    }
                    body.load(option.url);
                } else {
                    if (!page) {
                        body.css({ padding: 0, overflow: 'hidden' });
                    }
                    $('<iframe>').attr('frameborder', '0').attr('src', option.url).appendTo(body);
                }
            } else if (option.component) {
                var router = new Router(body);
                router.route(option);
            }
        }

        function createFooter(content) {
            var footer = $('<div>').addClass('dialog-footer');
            if (option.footer) {
                footer.appendTo(content);
                parseDom(footer, option.footer);
            } else if (option.buttons) {
                footer.appendTo(content);
                $(option.buttons).each(function (i, b) {
                    $('<button>').addClass('ok').append(b.text).on('click', function () {
                        b.handler && b.handler.call(this, dlg);
                    }).appendTo(footer);
                });
                _this._createCancel(footer, close);
            } else {
                body.css({ bottom: 0 });
            }
        }

        function parseDom(dom, template) {
            if (typeof template === 'function') {
                template(dom);
            } else {
                dom.append(template);
            }
        }

        if (type === 'dialog') {
            var width = option.width || 500;
            var height = option.height || 300;
            layer.css({
                width: width + 'px', height: height + 'px',
                marginTop: -(height / 2) + 'px', marginLeft: -(width / 2) + 'px'
            });

            var content = $('<div>').addClass('dialog-content').appendTo(layer);
            //header
            var btnMax;
            if (option.title) {
                var icon = option.icon || 'fa fa-window-maximize';
                var header = $('<div>').addClass('dialog-header');
                $('<i>').addClass('icon ' + icon).appendTo(header);
                header.append(option.title);
                setHeadMouseEvent(header);
                if (option.showMax) {
                    btnMax = createMaximizeIcon(header);
                }
                createCloseIcon(header);
                content.append(header);
            } else if (option.showClose) {
                createCloseIcon(layer);
            }
            //body
            var body = $('<div>').addClass('dialog-body').appendTo(content);
            if (!option.title) {
                body.css({ top: 0 });
            }
            createContent(body, false);
            //footer
            createFooter(content);

            if (option.max && btnMax) {
                btnMax.click();
            }
        } else {
            var content = $('<div>').addClass('page-content').appendTo(layer);
            createCloseIcon(layer);
            createContent(content, true);
        }

        option.success && option.success();
        return dlg;
    }

};