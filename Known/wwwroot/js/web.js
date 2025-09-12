function KScanner(invoker, videoId) {
    var deviceId;
    let reader = new ZXing.BrowserMultiFormatReader();
    reader.getVideoInputDevices().then(devices => deviceId = devices[0].deviceId).catch(error => {
        console.log(error);
        invoker.invokeMethodAsync('OnError', error.toString());
    });

    const vibrate = () => {
        if ('vibrate' in window.navigator) {
            window.navigator.vibrate([200, 100, 200]);
            const handler = window.setTimeout(function () {
                window.clearTimeout(handler);
                window.navigator.vibrate([]);
            }, 1000);
        }
    }

    this.start = function () {
        try {
            reader.decodeFromVideoDevice(deviceId, videoId, (res, err) => {
                if (res) vibrate();
                var text = res ? res.text : '';
                var error = err && !(err instanceof ZXing.NotFoundException) ? err : '';
                if (text.length || error.length) {
                    invoker.invokeMethodAsync('OnScanned', text, error);
                }
            });
        } catch (error) {
            console.log(error);
            invoker.invokeMethodAsync('OnError', error);
        }
    }

    this.stop = function () {
        reader.reset();
        invoker.invokeMethodAsync('OnScanStop');
    }
}

window.KUtils = {
    scanner: null,
    scanStart: function (invoker, videoId) {
        this.scanner = new KScanner(invoker, videoId);
        this.scanner.start();
    },
    scanStop: function () {
        this.scanner.stop();
    },
    scrollToBottom: function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.scrollTop = el.scrollHeight;
        }
    },
    highlight: function (code, lang) {
        return Prism.highlight(code, Prism.languages[lang], lang);
    }
};

window.isMobile = function () {
    const userAgent = navigator.userAgent || navigator.vendor || window.opera;
    return /android|iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
};

$(function () {
    window.Prism = window.Prism || {};
    Prism.disableWorkerMessageHandler = true;
    Prism.manual = true;
});