window.KForm = {
    init: function () {
        var inputs = $('.form input');
        if (inputs.length) {
            inputs.keydown(function (event) {
                if ((event.keyCode || event.which) === 13) {
                    event.preventDefault();
                    var index = inputs.index(this);
                    if (index < inputs.length - 1)
                        inputs[index + 1].focus();
                    this.blur();
                    var method = $(this).attr("onenter");
                    if (method && method.length)
                        eval(method);
                }
            });
            inputs[0].focus();
        }
        KAdmin.setFormList();
    },
    captcha: function (id, code) {
        var canvas = document.getElementById(id);
        var ctx = canvas.getContext("2d");
        var width = ctx.canvas.width;
        var height = ctx.canvas.height;
        ctx.clearRect(0, 0, width, height);
        ctx.lineWidth = 2;
        for (var i = 0; i < 1000; i++) {
            ctx.beginPath();
            var x = getRandom(width - 2);
            var y = getRandom(height - 2);
            ctx.moveTo(x, y);
            ctx.lineTo(x + 1, y + 1);
            ctx.strokeStyle = getColor();
            ctx.stroke();
        }
        for (var i = 0; i < 20; i++) {
            ctx.beginPath();
            var x = getRandom(width - 2);
            var y = getRandom(height - 2);
            var w = getRandom(width - x);
            var h = getRandom(height - y);
            ctx.moveTo(x, y);
            ctx.lineTo(x + w, y + h);
            ctx.strokeStyle = getColor();
            ctx.stroke();
        }
        ctx.font = width / 5 + 'px Î¢ÈíÑÅºÚ';
        ctx.textBaseline = 'middle';
        var codes = code.split('');
        for (var i = 0; i < codes.length; i++) {
            ctx.beginPath();
            ctx.fillStyle = '#f00';
            var word = codes[i];
            var w = width / codes.length;
            var left = getRandom(i * w, (i + 1) * w - width / 5);
            var top = getRandom(height / 2 - 10, height / 2 + 10);
            ctx.fillText(word, left, top);
        }

        function getRandom(a, b = 0) {
            var max = a;
            var min = b;
            if (a < b) {
                max = b;
                min = a;
            }
            return Math.floor(Math.random() * (max - min)) + min;
        }

        function getColor() {
            return `rgb(${Math.floor(Math.random() * 255)},${Math.floor(Math.random() * 256)},${Math.floor(Math.random() * 256)})`;
        }
    }
};