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

class PDAScanner {
    constructor(invoker, input) {
        this.invoker = invoker;
        this.input = input;
        this.buffer = '';
        this.scanTimer = null;
        this.lastKeyTime = 0;
        this.isActive = false;
        this.keepFocusInterval = null;
        this.handleKeyDown = this.handleKeyDown.bind(this);
        this.handleFocus = this.handleFocus.bind(this);
        this.stop();
    }

    start() {
        if (this.isActive) return;
        this.isActive = true;
        this.buffer = '';
        document.addEventListener('keydown', this.handleKeyDown);
        this.input.addEventListener('focus', this.handleFocus);
        this.keepFocus();
        this.keepFocusInterval = setInterval(() => this.keepFocus(), 1000);
        console.log('PDA扫码器已启动');
    }

    stop() {
        if (!this.isActive) return;
        this.isActive = false;
        this.buffer = '';
        document.removeEventListener('keydown', this.handleKeyDown);
        this.input.removeEventListener('focus', this.handleFocus);
        if (this.scanTimer) {
            clearTimeout(this.scanTimer);
            this.scanTimer = null;
        }
        if (this.keepFocusInterval) {
            clearInterval(this.keepFocusInterval);
            this.keepFocusInterval = null;
        }
        console.log('PDA扫码器已停止');
    }

    handleKeyDown(e) {
        if (!this.isActive) return;
        const now = Date.now();
        const isEnter = e.key === 'Enter' || e.keyCode === 13;
        if (isEnter) {
            e.preventDefault();
        }
        // 判断是否是新的一次扫码
        if (now - this.lastKeyTime > 500) {
            this.buffer = '';
        }
        // 记录按键时间
        this.lastKeyTime = now;
        // 处理按键
        if (!isEnter) {
            this.buffer += e.key;
        } else if (this.buffer) {
            this.processScan(this.buffer);
            this.buffer = '';
        }
        // 设置超时自动处理（应对无回车结束的扫码枪）
        if (this.scanTimer)
            clearTimeout(this.scanTimer);
        this.scanTimer = setTimeout(() => {
            if (this.buffer && this.buffer.length > 0) {
                this.processScan(this.buffer);
                this.buffer = '';
            }
        }, 500);
    }

    handleFocus() {
        if (!this.isActive) return;
        // 立即失去焦点，防止键盘弹出
        setTimeout(() => this.input.blur(), 0);
    }

    processScan(text) {
        if (!text || !this.isActive) return;
        console.log('扫码结果:', text);
        this.invoker.invokeMethodAsync('OnScanned', text, '');
        // 触发自定义事件
        //const event = new CustomEvent('scan', { detail: { data } });
        //document.dispatchEvent(event);
        setTimeout(() => this.keepFocus(), 100);
    }

    keepFocus() {
        if (!this.isActive) return;
        try {
            this.input.focus();
        } catch (e) {
            console.warn('无法获取焦点:', e);
        }
    }
}

window.KUtils = {
    pda: null,
    scanner: null,
    scanPDA: function (invoker, input) {
        this.pda = new PDAScanner(invoker, input);
        this.pda.start();
    },
    stopPDA: function () {
        this.pda?.stop();
    },
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
            el.scrollTo({ top: 0, behavior: 'smooth' });
        }
    },
    scrollToBottom: function (id) {
        var el = document.getElementById(id);
        if (el) {
            el.scrollTo({ top: el.scrollHeight, behavior: 'smooth' });
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