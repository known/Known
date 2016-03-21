$.extend({
    slide: function (container, contentCls, triggerCls, config) {
        // 外围盒子
        var container_box = $(container),
            // 内容父元素
            content_box = $(contentCls, container),
            // 触发器父元素
            trigger_box = $(triggerCls, container),
            // 内容子元素集合
            content_childs = $(contentCls, container).children(),
            // 第一个内容子元素
            first_CtnElem = $(content_childs.get(0)),
            // 触发器子元素集合
            trigger_childs = $(triggerCls, container).children(),
            // 滚动步伐值(固定，每个内容子元素宽度相同)
            fixed_steps = first_CtnElem.width(),
            // 内容子元素长度
            len = content_childs.length,
            // 内容子元素总长度
            total_w = fixed_steps * len,
            // 初始化left变量
            left = 0,
            // 定时器
            setTimeoutId,
            // 默认配置
            defaultConfig = {
                auto: false,
                delay: 4000,
                interval: 4400,
                duration: 400,
                easing: 'swing',
                activeCls: 'slide-active'
            };
        // 配置项
        config = $.isPlainObject(config) && config ? $.extend(defaultConfig, config) && defaultConfig : defaultConfig;

        // 定时器绑定函数
        function autoSlide() {
            left = parseInt(content_box.get(0).style.left, 10);
            left = left - fixed_steps; // 获取下一次轮播left值

            // 为当前触发器元素增加激活样式
            var curIndex = Math.abs(left / fixed_steps);
            if (curIndex <= len - 1) {
                $(trigger_childs[curIndex]).addClass(config.activeCls).siblings().removeClass(config.activeCls);
            } else {
                $(trigger_childs[0]).addClass(config.activeCls).siblings().removeClass(config.activeCls);
            }

            // 轮播计算
            if (Math.abs(left) < total_w) {
                content_box.stop(true, true).animate({ left: left + 'px' }, config.duration, config.easing, function () {
                    // 轮播到最后一项时，第一个内容子元素更新position，left属性值
                    if (Math.abs(left) === (total_w - fixed_steps)) {
                        first_CtnElem.css({ position: 'relative', left: total_w + 'px' });
                    }
                });
            }
            else {
                // 动画left为负的total_w像素时，重置第一个内容子元素和其自身样式
                content_box.stop(true, true).animate({ left: left + 'px' }, config.duration, config.easing, function () {
                    //first_CtnElem.css({ position: '', left: '' });
                    first_CtnElem.removeAttr('style');
                    content_box.css({ left: 0 });
                });
            }
            // 定时器
            if (!!config.autoplay) {
                setTimeoutId = setTimeout(autoSlide, config.delay);
            }
        }

        // 初始化操作
        content_box.css({ width: total_w + 'px', position: 'absolute', top: 0, left: 0 });

        // 是否自动轮播
        if (!!config.autoplay) {
            setTimeoutId = setTimeout(autoSlide, config.delay);
        }

        // 触发器绑定事件
        $(trigger_childs).bind('mouseover', function (e) {
            var curTarget = e.currentTarget,
                idx = trigger_childs.index(curTarget);
            left = -(total_w / len) * idx;

            $(curTarget).addClass(config.activeCls).siblings().removeClass(config.activeCls);
            content_box.stop(true, true).animate({ left: left + 'px' }, config.duration, config.easing, function () {
                //first_CtnElem.css({ position: '', left: '' });
                first_CtnElem.removeAttr('style');
                // 轮播到最后一项时，第一个内容子元素更新position，left属性值
                if (Math.abs(left) === (total_w - fixed_steps)) {
                    first_CtnElem.css({ position: 'relative', left: total_w + 'px' });
                }
            });
        });

        // 外围盒子绑定事件
        // 触发mouseover事件，取消在各种情景中正在执行的定时器，避免与触发器元素所触发的事件代码冲突
        container_box.bind('mouseover', function (e) {
            // 第一次触发mouseover事件时，如果延迟时间还没过时，则取消定时器setTimeoutId
            // 第一次触发mouseover事件时，如果延迟已过，则取消正在执行的定时器setTimeoutId
            // 如果超过一次触发mouseover事件时，则取消由mouseleave事件所触发的定时器
            if (!!config.autoplay) {
                clearTimeout(setTimeoutId);
            }
        });

        // 触发mouseleave事件，增加定时器，自动轮播得以继续
        container_box.bind('mouseleave', function (e) {
            if (!!config.autoplay) {
                setTimeoutId = setTimeout(autoSlide, config.delay);
            }
        });

        return this;
    }
});

$.slide('#slider', '.content', '.trigger', {
    autoplay: true,
    auto: true,
    delay: 3000,
    interval: 3000,
    duration: 500,
    easing: 'linear',
    activeCls: 'current'
});
