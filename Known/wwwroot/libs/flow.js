function FlowArrow(x1, y1, x2, y2) {
    this.x1 = x1;
    this.y1 = y1;
    this.x2 = x2;
    this.y2 = y2;
    this.tmpX1 = null;
    this.tmpY1 = null;
    this.tmpX2 = null;
    this.tmpY2 = null;
    this.color = "black";

    this.setColor = function (color) {
        this.color = color;
    }

    this.setP = function (x1, y1, x2, y2) {
        this.x1 = x1;
        this.y1 = y1;
        this.x2 = x2;
        this.y2 = y2;
    }

    this.setP1 = function (tmpX1, tmpY1) {
        this.tmpX1 = tmpX1;
        this.tmpY1 = tmpY1;
    }

    this.setP2 = function (tmpX2, tmpY2) {
        this.tmpX2 = tmpX2;
        this.tmpY2 = tmpY2;
    }

    this.drawBottomToTop = function (ctx, option) {
        if (this.x1 != this.x2) {
            this.setP1(this.x1, (this.y1 + this.y2) / 2);
            this.setP2(this.x2, (this.y1 + this.y2) / 2);
            this.draw(ctx);
        } else {
            this.draw(ctx);
        }

        if (option && option.text) {
            ctx.fillText(option.text, this.x1 + 1, (this.y1 + this.y2) / 2);
        }
    }

    this.drawBottomToRight = function (ctx, option) {
        if (this.y1 != this.y2) {
            this.setP1(this.x1, this.y2);
            this.draw(ctx);
        } else {
            this.draw(ctx);
        }

        if (option && option.text) {
            ctx.fillText(option.text, this.x1 + 1, (this.y1 + this.y2) / 2);
        }
    }

    this.drawLeftOrRightToTop = function (ctx, option) {
        this.setP1(this.x2, this.y1);
        this.draw(ctx);

        if (option && option.text) {
            ctx.fillText(option.text, this.x2 + 1, (this.y1 + this.y2) / 2);
        }
    }

    this.drawLeftToRightOrRightToLeft = function (ctx, option) {
        option = option || {};
        if (this.y1 != this.y2) {
            var offsetX = option.offsetX || 0;
            this.setP1((this.x1 + this.x2 + offsetX) / 2, this.y1);
            this.setP2((this.x1 + this.x2 + offsetX) / 2, this.y2);
            this.draw(ctx);
            if (option.text) {
                ctx.fillText(option.text, (this.x1 + this.x2 + offsetX) / 2 + 1, (this.y1 + this.y2) / 2);
            }
        } else {
            this.draw(ctx);
            if (option.text) {
                ctx.fillText(option.text, (this.x1 + this.x2) / 2 + 1, (this.y1 + this.y2) / 2 + 15);
            }
        }
    }

    this.drawRightToLeftOrLeftToRight = function (ctx, option) {
        option = option || {};
        if (this.y1 != this.y2) {
            var offsetX = option.offsetX || 0;
            this.setP1((this.x1 + this.x2 + offsetX) / 2, this.y1);
            this.setP2((this.x1 + this.x2 + offsetX) / 2, this.y2);
            this.draw(ctx);
            if (option && option.text) {
                ctx.fillText(option.text, (this.x1 + this.x2 + offsetX) / 2 + 1, (this.y1 + this.y2) / 2);
            }
        } else {
            this.draw(ctx);
            if (option.text) {
                ctx.fillText(option.text, (this.x1 + this.x2) / 2 + 1, (this.y1 + this.y2) / 2 + 15);
            }
        }
    }

    this.draw = function (ctx) {
        // arbitrary styling
        ctx.strokeStyle = this.color;
        ctx.fillStyle = this.color;
        // draw the line
        ctx.beginPath();
        ctx.moveTo(this.x1, this.y1);
        if (this.tmpX1 != null && this.tmpY1 != null && this.tmpX2 != null && this.tmpY2 != null) {
            ctx.lineTo(this.tmpX1, this.tmpY1);
            ctx.closePath();
            ctx.stroke();
            ctx.beginPath();
            ctx.moveTo(this.tmpX1, this.tmpY1)
            ctx.lineTo(this.tmpX2, this.tmpY2);
            ctx.closePath();
            ctx.stroke();
            ctx.beginPath();
            ctx.moveTo(this.tmpX2, this.tmpY2);
            ctx.lineTo(this.x2, this.y2);
            ctx.closePath();
            ctx.stroke();
            var endRadians = Math.atan((this.y2 - this.tmpY2) / (this.x2 - this.tmpX2));
            endRadians += ((this.x2 >= this.tmpX2) ? 90 : -90) * Math.PI / 180;
            this.drawArrowHead(ctx, this.x2, this.y2, endRadians);
        } else if (this.tmpX1 != null && this.tmpY1 != null && this.tmpX2 == null && this.tmpY2 == null) {
            ctx.lineTo(this.tmpX1, this.tmpY1);
            ctx.closePath();
            ctx.stroke();
            ctx.beginPath();
            ctx.moveTo(this.tmpX1, this.tmpY1)
            ctx.lineTo(this.x2, this.y2);
            ctx.closePath();
            ctx.stroke();
            var endRadians = Math.atan((this.y2 - this.tmpY1) / (this.x2 - this.tmpX1));
            endRadians += ((this.x2 >= this.tmpX1) ? 90 : -90) * Math.PI / 180;
            this.drawArrowHead(ctx, this.x2, this.y2, endRadians);
        } else if (this.tmpX1 == null && this.tmpY1 == null && this.tmpX2 == null && this.tmpY2 == null) {
            ctx.lineTo(this.x2, this.y2);
            ctx.closePath();
            ctx.stroke();
            var endRadians = Math.atan((this.y2 - this.y1) / (this.x2 - this.x1));
            endRadians += ((this.x2 >= this.x1) ? 90 : -90) * Math.PI / 180;
            this.drawArrowHead(ctx, this.x2, this.y2, endRadians);
        }
    }

    this.drawArrowHead = function (ctx, x, y, radians) {
        ctx.save();
        ctx.beginPath();
        ctx.translate(x, y);
        ctx.rotate(radians);
        ctx.moveTo(0, 0);
        ctx.lineTo(5, 10);
        ctx.lineTo(-5, 10);
        ctx.closePath();
        ctx.restore();
        ctx.fill();
    }
}

//step
function FlowStep(chart, ctx, item) {
    var x = item.x, y = item.y;
    this.id = item.id;
    this.name = item.name;
    this.item = item;
    this.flag = item.flag;
    this.chart = chart;
    this.l = 30;
    this.h = 50;
    this.w = 2 * this.h;
    this.x = x;
    this.y = y;

    if (item.round) {
        drawRoundRect(ctx, x - this.w / 2, y - this.h / 2, this.w, this.h, item.name);
    } else if (item.flag === 'condition') {
        drawRhombus(ctx, x, y, this.l, item.name);
    } else {
        ctx.strokeRect(x - this.w / 2, y - this.h / 2, this.w, this.h);
        ctx.fillText(item.name, x - this.w / 2 + 10, y + 5);
    }

    this.contains = function (x, y) {
        if (this.flag === 'condition') {
            return x >= this.x - this.l * 2 && y >= this.y - this.l && x <= (this.x + this.l * 2) && y <= (this.y + this.l * 2);
        }

        var x1 = this.x - this.w / 2;
        var y1 = this.y - this.h / 2;
        return x >= x1 && y >= y1 && x <= (x1 + this.w) && y <= (y1 + this.h);
    }

    this.drawBottomToTop = function (obj, option) {
        obj = getStepObject(obj);
        var y = this.flag === 'step' ? this.h / 2 : this.l;
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x, this.y + y, obj.x, obj.y - obj.h / 2);
            arrow.drawBottomToTop(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x, this.y + y, obj.x, obj.y - obj.l);
            arrow.drawBottomToTop(ctx, option);
        }
    }

    this.drawBottomToLeft = function (obj, option) {
        obj = getStepObject(obj);
        if (obj.flag == "step") {
            arrow = new Arrow(this.x, this.y + this.h / 2, obj.x - obj.w / 2, obj.y);
            arrow.drawBottomToRight(ctx, option);
        } else if (obj.flag == "condition") {
            arrow = new Arrow(this.x, this.y + this.h / 2, obj.x - obj.l * 2 - 40, obj.y);
            arrow.drawBottomToRight(ctx, option);
        }
    }

    this.drawBottomToRight = function (obj, option) {
        obj = getStepObject(obj);
        if (obj.flag == "step") {
            arrow = new Arrow(this.x, this.y + this.h / 2, obj.x + obj.w / 2, obj.y);
            arrow.drawBottomToRight(ctx, option);
        } else if (obj.flag == "condition") {
            arrow = new Arrow(this.x, this.y + this.h / 2, obj.x + obj.l * 2 + 40, obj.y);
            arrow.drawBottomToRight(ctx, option);
        }
    }

    this.drawLeftToTop = function (obj, option) {
        obj = getStepObject(obj);
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x - this.w * 2, this.y, obj.x, obj.y - obj.h / 2);
            arrow.drawLeftOrRightToTop(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x - this.l * 2, this.y, obj.x, obj.y - obj.l);
            arrow.drawLeftOrRightToTop(ctx, option);
        }
    }

    this.drawLeftToLeft = function (obj, option) {
        obj = getStepObject(obj);
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x - this.w * 2, this.y, obj.x + this.w / 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x - this.l * 2, this.y, obj.x + this.l * 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        }
    }

    this.drawLeftToRight = function (obj, option) {
        obj = getStepObject(obj);
        var x = this.flag === 'step' ? this.w / 2 : this.l * 2;
        option = option || {};
        option.offsetX = this.flag === 'step' ? 0 : 100;
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x - x, this.y, obj.x + this.w / 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x - x, this.y, obj.x + this.l * 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        }
    }

    this.drawRightToTop = function (obj, option) {
        obj = getStepObject(obj);
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x + this.w * 2, this.y, obj.x, obj.y - obj.h / 2);
            arrow.drawLeftOrRightToTop(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x + this.l * 2, this.y, obj.x, obj.y - obj.l);
            arrow.drawLeftOrRightToTop(ctx, option);
        }
    }

    this.drawRightToLeft = function (obj, option) {
        obj = getStepObject(obj);
        var x = this.flag === 'step' ? this.w / 2 : this.l * 2;
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x + x, this.y, obj.x - this.w / 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x + x, this.y, obj.x - this.l * 2, obj.y);
            arrow.drawLeftToRightOrRightToLeft(ctx, option);
        }
    }

    this.drawRightToRight = function (obj, option) {
        obj = getStepObject(obj);
        var x = this.flag === 'step' ? this.w / 2 : this.l * 2;
        option = option || {};
        option.offsetX = 50;
        if (obj.flag == "step") {
            var arrow = new FlowArrow(this.x + x, this.y, obj.x + this.w / 2, obj.y);
            arrow.drawRightToLeftOrLeftToRight(ctx, option);
        } else if (obj.flag == "condition") {
            var arrow = new FlowArrow(this.x + x, this.y, obj.x + this.l * 2, obj.y);
            arrow.drawRightToLeftOrLeftToRight(ctx, option);
        }
    }

    function getStepObject(obj) {
        if (typeof obj === 'string') {
            var objs = chart.steps.filter(function (s) {
                return s.id === obj;
            });
            obj = objs.length > 0 ? objs[0] : null;
        }

        return obj;
    }

    function drawRoundRect(ctx, x, y, w, h, text) {
        var r = h / 2;
        ctx.beginPath();
        ctx.moveTo(x + r, y);
        ctx.arcTo(x + w, y, x + w, y + h, r);
        ctx.arcTo(x + w, y + h, x, y + h, r);
        ctx.arcTo(x, y + h, x, y, r);
        ctx.arcTo(x, y, x + w, y, r);
        ctx.closePath();
        var mt = ctx.measureText(text);
        var fix = mt.actualBoundingBoxAscent + mt.actualBoundingBoxDescent;
        ctx.fillText(text, x + w / 2 - mt.width / 2, y + h / 2 + fix / 2);
        ctx.stroke();
    }

    function drawRhombus(ctx, x, y, l, text) {
        ctx.beginPath();
        ctx.moveTo(x, y + l);
        ctx.lineTo(x - l * 2, y);
        ctx.lineTo(x, y - l);
        ctx.lineTo(x + l * 2, y);
        ctx.closePath();
        var mt = ctx.measureText(text);
        var fix = mt.actualBoundingBoxAscent + mt.actualBoundingBoxDescent;
        ctx.fillText(text, x - mt.width / 2, y + fix / 2);
        ctx.stroke();
    }
}

function FlowChart(canvasId, option) {
    var _this = this;

    this.option = option;
    this.steps = [];

    this.render = function (elem) {
        var width = option.width || 800;
        var height = option.height || 600;
        var font = option.font || 'normal 14px YaHei';
        elem.html('');
        $('<canvas>')
            .attr('id', canvasId)
            .attr('width', width)
            .attr('height', height)
            .appendTo(elem);

        var canvas = document.getElementById(canvasId);
        var ctx = canvas.getContext('2d');
        ctx.font = font;

        for (var i = 0; i < option.steps.length; i++) {
            var item = option.steps[i];
            var step = new FlowStep(_this, ctx, item);
            _this.steps.push(step);
        }

        for (var i = 0; i < option.steps.length; i++) {
            var item = option.steps[i];
            var step = _this.steps[i];
            item.arrow && eval(item.arrow);
        }

        if (option.click) {
            canvas.onclick = function (e) {
                e = e || window.event;
                var x = e.offsetX, y = e.offsetY;
                var name = '', step = null;
                for (var i = 0; i < _this.steps.length; i++) {
                    var item = _this.steps[i];
                    if (item.contains(x, y)) {
                        name = item.name;
                        step = item;
                        break;
                    }
                }
                option.click({ name: name, chart: _this, step: step });
            }
        }
    }
}

window.Flowcharts = {
    chart: function (info) {
        console.log(info);
        var canvasId = info.id + 'Canvas';
        var elem = $('#' + info.id);
        var chart = new FlowChart(canvasId, info);
        chart.render(elem);
    }
}