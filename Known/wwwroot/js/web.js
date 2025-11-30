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
    scrollToTop: function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.scrollTop = 0;
        }
    },
    scrollToBottom: function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.scrollTop = el.scrollHeight;
        }
    },
    getUserAgent: function () {
        return navigator.userAgent;
    },
    isMobile: function () {
        const userAgent = navigator.userAgent || navigator.vendor || window.opera;
        return /android|iPad|iPhone|iPod/.test(userAgent) && !window.MSStream;
    },
    highlight: function (code, lang) {
        return Prism.highlight(code, Prism.languages[lang], lang);
    },
    showECharts: function (elementId, option) {
        var myChart = echarts.init(document.getElementById(elementId));
        myChart.setOption(option);
        window.addEventListener("resize", function () { myChart.resize(); });
        return myChart;
    },
    checkClipboardPermission: async function () {
        if (navigator.permissions && navigator.permissions.query) {
            const { state } = await navigator.permissions.query({ name: 'clipboard-read' });
            if (state === 'prompt' || state === 'denied') {
                try {
                    await navigator.clipboard.read();
                } catch (error) {
                    console.warn('Clipboard access denied:', error);
                }
            }
        }
    },
    setupPasteListener: function (invoker, element) {
        element.addEventListener('paste', async (event) => {
            const clipboardItems = event.clipboardData.items;
            for (const item of clipboardItems) {
                if (item.type.indexOf('image') !== -1) {
                    event.preventDefault();
                    const blob = item.getAsFile();
                    const reader = new FileReader();
                    reader.onload = function () {
                        const base64Data = reader.result.split(',')[1]; // 移除 data URL 前缀
                        invoker.invokeMethodAsync('ReceivePastedImage', base64Data);
                    };
                    reader.readAsDataURL(blob);
                    break;
                }
            }
        });
    }
};

window.KNotify = {
    conn: null,
    handlers: new Map(),
    init: function (invoker, info) {
        const connection = new signalR.HubConnectionBuilder().withUrl(info.notifyUrl).build();
        const forceLogoutHandler = message => invoker.invokeMethodAsync(info.showForceLogout, message);
        const notifyLayoutHandler = message => invoker.invokeMethodAsync(info.showNotify, message);
        connection.on(info.forceLogout, forceLogoutHandler);
        connection.on(info.notifyLayout, notifyLayoutHandler);
        this.handlers.set(info.forceLogout, forceLogoutHandler);
        this.handlers.set(info.notifyLayout, notifyLayoutHandler);
        connection.start().then(function () {
            console.log("SignalR连接已建立");
            const sessionId = sessionStorage.getItem('sessionId');
            if (sessionId) {
                connection.invoke("RegisterSession", sessionId);
            }
        }).catch(function (err) {
            console.error("SignalR连接错误: " + err.toString());
        });
        this.conn = connection;
    },
    addSession: function (sessionId) {
        if (sessionId) {
            sessionStorage.setItem("sessionId", sessionId);
        }
    },
    register: function (invoker, method, invoke) {
        const handler = message => invoker.invokeMethodAsync(invoke, message);
        this.conn?.on(method, handler);
        this.handlers.set(method, handler);
    },
    close: function (method) {
        const handler = this.handlers.get(method);
        if (handler && this.conn) {
            this.conn.off(method, handler);
            this.handlers.delete(method);
        }
    },
    closeAll: function () {
        for (const [method, handler] of this.handlers) {
            this.conn?.off(method, handler);
        }
        this.handlers.clear();
    },
    dispose: function () {
        this.closeAll();
        if (this.conn) {
            this.conn.stop();
            this.conn = null;
            console.log("SignalR连接已停止");
        }
    }
};

let pwaInitialized = false;
function initializePwaInstallButton(btnId) {
    if (pwaInitialized) return;
    pwaInitialized = true;

    const installButton = document.getElementById(btnId);
    const isIOS = /iPad|iPhone|iPod/.test(navigator.userAgent) && !window.MSStream;
    if ('serviceWorker' in navigator && 'onbeforeinstallprompt' in window) {
        if (isIOS || !('onbeforeinstallprompt' in window)) {
            installButton.style.display = 'none';
            document.getElementById('iosInstallGuide').style.display = 'block';
        }
    }
    window.addEventListener('load', () => {
        if (window.matchMedia('(display-mode: standalone)').matches) {
            installButton.style.display = 'none';
        }
    });
}

$(function () {
    window.Prism = window.Prism || {};
    Prism.disableWorkerMessageHandler = true;
    Prism.manual = true;
});