Date.prototype.format = function (fmt) {
    var o = {
        'M+': this.getMonth() + 1,//月份   
        'd+': this.getDate(),//日   
        'h+': this.getHours(),//小时  
        'H+': this.getHours(),
        'm+': this.getMinutes(),//分   
        's+': this.getSeconds(),//秒   
        'q+': Math.floor((this.getMonth() + 3) / 3), //季度   
        'S': this.getMilliseconds()//毫秒   
    };
    if (/(y+)/.test(fmt)) {
        fmt = fmt.replace(RegExp.$1, (this.getFullYear() + '').substr(4 - RegExp.$1.length));
    }
    for (var k in o) {
        if (new RegExp('(' + k + ')').test(fmt)) {
            fmt = fmt.replace(RegExp.$1, (RegExp.$1.length === 1)
                ? o[k]
                : ('00' + o[k]).substr(('' + o[k]).length));
        }
    }
    return fmt;
};

var Utils = {

    list2Tree: function (data, key, pid) {
        var list = data.slice();
        return list.filter(function (parent) {
            var branchArr = list.filter(function (child) {
                return parent.id === child[key];
            });
            if (branchArr.length > 0) {
                parent.children = branchArr;
            }
            return parent[key] === pid;
        });
    },

    tree2List: function (data, key) {
        var tree = data.slice();
        return tree.reduce(function (con, item) {
            var callee = arguments.callee;
            con.push(item);
            if (item[key] && item[key].length > 0)
                item[key].reduce(callee, con);
            return con;
        }, []).map(function (item) {
            item[key] = [];
            return item;
        });
    },

    setUser: function (user) {
        sessionStorage.setItem('Known_User', JSON.stringify(user));
    },

    getUser: function () {
        var value = sessionStorage.getItem('Known_User');
        if (!value)
            return null;

        return JSON.parse(value);
    },

    setCodes: function (data) {
        localStorage.setItem('Known_Codes', JSON.stringify(data));
    },

    getCodes: function (category) {
        var value = localStorage.getItem('Known_Codes');
        if (!value)
            return null;

        var codes = JSON.parse(value);
        if (!codes)
            return null;

        return codes[category];
    },

    setFormDetail: function (container, data) {
        for (var p in data) {
            var elem = container.find('.' + p);
            if (elem.length) {
                var dateFormat = elem.attr('dateFormat');
                if (dateFormat) {
                    elem.html(new Date(data[p]).format(dateFormat));
                } else {
                    elem.html(data[p]);
                }
            }
        }
    },

    bindList: function (select, datas, emptyText) {
        if (emptyText === undefined) {
            emptyText = '请选择';
        }
        if (emptyText !== '') {
            select.append('<option value="" selected>' + emptyText + '</option>');
        }

        if (datas && datas.length) {
            for (var i = 0; i < datas.length; i++) {
                var code = datas[i].Code;
                var name = datas[i].Name || code;
                var selected = emptyText || i !== 0 ? '' : ' selected'
                select.append('<option value="' + code + '"' + selected + '>' + name + '</option>');
            }
        }
    }

}

$(document).ajaxComplete(function (evt, xhr, settings) {
    if (xhr.responseText && xhr.responseText.match("^\{(.+:.+,*){1,}\}$")) {
        var result = JSON.parse(xhr.responseText);
        if (result && result.timeout) {
            top.location = '/login';
        }
    }
});

$(function () {
    $('[url]').each(function () {
        var $this = $(this).html(''), url = $this.attr('url');
        if (url) {
            $.get(url, function (res) {
                if ($this[0].tagName === 'SELECT') {
                    var emptyText = $this.attr('emptyText')
                    Utils.bindList($this, res, emptyText);
                }
            });
        }
    });

    $('[code]').each(function () {
        var $this = $(this).html(''), code = $this.attr('code'),
            emptyText = $this.attr('emptyText');
        var codes = Utils.getCodes(code);
        Utils.bindList($this, codes, emptyText);
    });
});

$(document).on("click", ".layui-table-body table.layui-table tbody tr", function () {
    var index = $(this).attr('data-index');
    var tableBox = $(this).parents('.layui-table-box');
    //存在固定列
    if (tableBox.find(".layui-table-fixed.layui-table-fixed-l").length > 0) {
        tableDiv = tableBox.find(".layui-table-fixed.layui-table-fixed-l");
    } else {
        tableDiv = tableBox.find(".layui-table-body.layui-table-main");
    }
    //获取已选中列并取消选中
    var trs = tableDiv.find(".layui-unselect.layui-form-checkbox.layui-form-checked").parent().parent().parent();
    for (var i = 0; i < trs.length; i++) {
        var ind = $(trs[i]).attr("data-index");
        if (ind != index) {
            var checkCell = tableDiv.find("tr[data-index=" + ind + "]").find("td div.laytable-cell-checkbox div.layui-form-checkbox I");
            if (checkCell.length > 0) {
                checkCell.click();
            }
        }
    }
    //选中单击行
    var checkCell = tableDiv.find("tr[data-index=" + index + "]").find("td div.laytable-cell-checkbox div.layui-form-checkbox I");
    if (checkCell.length > 0 & trs.length === 1) {
        checkCell.click();
    } else {
        checkCell.click();
    }
});
$(document).on("click", "td div.laytable-cell-checkbox div.layui-form-checkbox", function (e) {
    e.stopPropagation();
});