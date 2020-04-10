layui.define(function (exports) {
    exports('helper', {
        fullScreen: function () {
            var el = document.documentElement;
            var rfs = el.requestFullScreen || el.webkitRequestFullScreen || el.mozRequestFullScreen || el.msRequestFullScreen;
            if (rfs) {
                rfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE，这里其实就是模拟了按下键盘的F11，使浏览器全屏
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
        },
        exitScreen: function () {
            var el = document;
            var cfs = el.cancelFullScreen || el.webkitCancelFullScreen || el.mozCancelFullScreen || el.exitFullScreen;
            if (cfs) {
                cfs.call(el);
            } else if (typeof window.ActiveXObject !== 'undefined') {
                //for IE，这里和fullScreen相同，模拟按下F11键退出全屏
                var wscript = new ActiveXObject('WScript.Shell');
                if (wscript !== null) {
                    wscript.SendKeys('{F11}');
                }
            }
        },
        toTree: function (arr, rootId) {
            arr.forEach(function (element) {
                var parentId = element.pid;
                if (parentId) {
                    arr.forEach(function (ele) {
                        if (ele.id === parentId) {
                            if (!ele.children) {
                                ele.children = [];
                            }
                            ele.children.push(element);
                        }
                    });
                }
            });
            arr = arr.filter(function (ele) {
                return ele.pid === rootId;
            });
            return arr;
        }
    });
});