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
    constructor(invoker) {
        this.invoker = invoker;
        this.input = null;

        this.isActive = false;
        this.keepFocusInterval = null;
        this.isProcessing = false;
        this.lastScanText = '';
        this.lastScanTime = 0;

        this.handleKeyDown = this.handleKeyDown.bind(this);
        this.handleFocus = this.handleFocus.bind(this);
        this.stop(); // 确保初始状态是停止的
    }

    start() {
        if (this.isActive) return;
        this.isActive = true;
        this.isProcessing = false;
        // 创建隐藏输入框
        this.createInput();
        console.log('PDA扫码器已启动');
    }

    stop() {
        if (!this.isActive) return;
        this.isActive = false;
        this.isProcessing = false;

        // 清除定时器
        if (this.keepFocusInterval) {
            clearInterval(this.keepFocusInterval);
            this.keepFocusInterval = null;
        }

        // 销毁隐藏输入框
        this.destroyInput();
        console.log('PDA扫码器已停止');
    }

    createInput() {
        // 如果已存在，先销毁
        if (this.input) {
            this.destroyInput();
        }

        // 创建隐藏输入框
        this.input = document.createElement('input');
        this.input.type = 'text';
        this.input.style.cssText = `
            position: fixed;
            top: -100px;
            left: -100px;
            width: 1px;
            height: 1px;
            opacity: 0;
            pointer-events: none;
        `;
        this.input.setAttribute('inputmode', 'none'); // 关键：阻止软键盘弹出
        document.body.appendChild(this.input);

        // 添加事件监听
        this.input.addEventListener('keydown', this.handleKeyDown);
        this.input.addEventListener('focus', this.handleFocus);
        this.input.addEventListener('blur', () => {
            if (this.isActive) {
                setTimeout(() => this.keepFocus(), 10);
            }
        });

        // 开始保持焦点
        this.keepFocus();
        this.keepFocusInterval = setInterval(() => this.keepFocus(), 1000);
    }

    destroyInput() {
        if (this.input) {
            // 移除事件监听
            this.input.removeEventListener('keydown', this.handleKeyDown);
            this.input.removeEventListener('focus', this.handleFocus);

            // 从DOM中移除
            if (this.input.parentNode) {
                this.input.parentNode.removeChild(this.input);
            }

            this.input = null;
        }
    }

    handleKeyDown(e) {
        if (!this.isActive || !this.input || this.isProcessing) return;

        // 检测Enter键（回车键）
        if (e.key === 'Enter' || e.keyCode === 13) {
            e.preventDefault(); // 阻止默认行为

            const scanText = this.input.value.trim();
            // 检查是否有有效数据
            if (scanText && scanText.length > 0) {
                this.processScan(scanText);
            }

            // 立即清空输入框
            this.input.value = '';
        }
    }

    handleFocus() {
        if (!this.isActive || !this.input) return;

        // 确保输入框是空的，准备接收新的扫描
        setTimeout(() => this.input.value = '', 0);
        this.isProcessing = false;
    }

    processScan(text) {
        if (!text || !this.isActive || this.isProcessing) return;

        // 设置处理标志，防止重复处理
        this.isProcessing = true;

        // 检查是否是重复扫码（简单实现）
        const now = Date.now();
        if (this.lastScanText === text && (now - this.lastScanTime) < 300) {
            console.log('检测到重复扫码，跳过:', text);
            this.isProcessing = false;
            return;
        }

        // 记录本次扫码
        this.lastScanText = text;
        this.lastScanTime = now;

        console.log('扫码结果:', text);

        // 调用外部方法
        try {
            this.invoker.invokeMethodAsync('OnScanned', text, '');
        } catch (error) {
            console.error('调用扫描回调失败:', error);
        }

        // 延迟重置处理标志，确保不会重复处理同一个扫码
        setTimeout(() => this.isProcessing = false, 100);
        // 重新获取焦点，准备下一次扫码
        setTimeout(() => this.keepFocus(), 50);
    }

    keepFocus() {
        if (!this.isActive || !this.input || this.isProcessing) return;
        try {
            // 隐藏输入框获取焦点，用于接收扫描输入
            this.input.focus();
        } catch (e) {
            console.warn('无法获取焦点:', e);
        }
    }

    // 添加一个重启方法
    restart() {
        this.stop();
        setTimeout(() => this.start(), 100);
    }

    // 添加一个销毁方法，用于完全清理
    destroy() {
        this.stop();
        // 移除所有绑定的事件和引用
        this.invoker = null;
        this.input = null;
    }
}

window.KUtils = {
    pda: null,
    scanner: null,
    reconnectObserver: null,
    reconnectTimer: null,
    reconnectLoginUrl: '/login',
    reconnectDelay: 5000,
    setupReconnectAutoLogin: function (loginUrl, delayMs) {
        this.reconnectLoginUrl = loginUrl || '/login';
        this.reconnectDelay = delayMs || 5000;

        const modal = document.getElementById('components-reconnect-modal');
        if (!modal) return;

        const scheduleRedirect = () => {
            if (this.reconnectTimer) return;
            this.reconnectTimer = window.setTimeout(() => {
                const current = location.pathname + location.search;
                const target = `${this.reconnectLoginUrl}?returnUrl=${encodeURIComponent(current)}`;
                location.href = target;
            }, this.reconnectDelay);
        };

        const clearRedirect = () => {
            if (this.reconnectTimer) {
                window.clearTimeout(this.reconnectTimer);
                this.reconnectTimer = null;
            }
        };

        const handleClass = () => {
            const cls = modal.className || '';
            // 仅在服务器明确拒绝重连时跳登录，避免长耗时操作导致的临时重连误判
            if (cls.includes('components-reconnect-rejected')) {
                scheduleRedirect();
            } else {
                clearRedirect();
            }
        };

        handleClass();
        if (this.reconnectObserver) {
            this.reconnectObserver.disconnect();
        }

        this.reconnectObserver = new MutationObserver(handleClass);
        this.reconnectObserver.observe(modal, { attributes: true, attributeFilter: ['class'] });
    },
    scanPDA: function (invoker, input) {
        this.pda = new PDAScanner(invoker, input);
        this.pda.start();
    },
    stopPDA: function () {
        this.pda?.destroy();
    },
    scanStart: function (invoker, videoId) {
        this.scanner = new KScanner(invoker, videoId);
        this.scanner.start();
    },
    scanStop: function () {
        this.scanner.stop();
    },
    runScript: function (script) {
        return eval(script);
    },
    runScriptVoid: function (script) {
        eval(script);
    },
    elemClick: function (id) {
        document.getElementById(id).click();
    },
    elemEnabled: function (id, enabled) {
        document.getElementById(id).enabled = enabled;
    },
    enterToTab: function (id) {
        var container = document.getElementById(id);
        if (!container) return;
        if (container.dataset.enterTab) return;
        container.dataset.enterTab = '1';

        container.addEventListener('keyup', function (e) {
            if (e.key !== 'Enter') return;

            var target = e.target;
            // 排除特殊元素
            //if (target.tagName === 'TEXTAREA') return;
            //if (target.type === 'checkbox' || target.type === 'radio' || target.type === 'submit' || target.type === 'button') return;
            //if (target.closest('button')) return;
            // 有业务OnEnter的输入框由C#处理跳转
            //if (target.closest('.kui-biz-enter')) return;
            // 不在表单区域内
            if (!target.closest('.kui-form')) return;

            // 收集所有可聚焦元素
            var selectors = [
                'input:not([type="hidden"]):not([type="submit"]):not([type="button"])',
                '.ant-select-selection-search-input',
                '.ant-picker input',
                '.ant-input-number-input'
            ];
            var all = container.querySelectorAll(selectors.join(','));
            var focusable = [];
            for (var i = 0; i < all.length; i++) {
                var el = all[i];
                if (!el.offsetParent || el.disabled || el.readOnly) continue;
                if (focusable.indexOf(el) >= 0) continue;
                focusable.push(el);
            }

            var idx = focusable.indexOf(target);
            if (idx >= 0 && idx < focusable.length - 1) {
                e.preventDefault();
                e.stopPropagation();
                var next = focusable[idx + 1];
                next.focus();
                try { next.select(); } catch (ex) { }
                next.dispatchEvent(new FocusEvent('focus', { bubbles: true }));
            }
        });
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
        var element = document.getElementById(elementId);
        if (!element)
            return null;

        var myChart = echarts.getInstanceByDom(element);
        if (!myChart)
            myChart = echarts.init(element);

        myChart.setOption(option, true);

        requestAnimationFrame(function () {
            requestAnimationFrame(function () {
                var chart = echarts.getInstanceByDom(element);
                if (chart)
                    chart.resize();
            });
        });

        if (!element.__kuiChartResizeBound) {
            element.__kuiChartResizeBound = true;
            if (window.ResizeObserver) {
                var observer = new ResizeObserver(function () {
                    var chart = echarts.getInstanceByDom(element);
                    if (chart)
                        chart.resize();
                });
                observer.observe(element);
                element.__kuiChartObserver = observer;
            } else {
                window.addEventListener("resize", function () {
                    var chart = echarts.getInstanceByDom(element);
                    if (chart)
                        chart.resize();
                });
            }
        }

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

window.KHotkey = {
    hotkeys: new Map(),
    hotkeyHandler: null,
    normalizeKey: function (key) {
        if (!key) return null;

        const value = key.toString().toLowerCase().trim();
        switch (value) {
            case 'ctrl':
            case 'control':
                return 'ctrl';
            case 'alt':
            case 'option':
                return 'alt';
            case 'shift':
                return 'shift';
            case 'meta':
            case 'cmd':
            case 'command':
            case 'win':
            case 'windows':
                return 'meta';
            case 'esc':
                return 'escape';
            case 'del':
                return 'delete';
            case 'return':
                return 'enter';
            case 'spacebar':
            case 'space':
                return ' ';
            case 'left':
                return 'arrowleft';
            case 'right':
                return 'arrowright';
            case 'up':
                return 'arrowup';
            case 'down':
                return 'arrowdown';
            case 'plus':
                return '+';
            default:
                return value;
        }
    },
    normalizeHotkey: function (hotkey) {
        if (!hotkey) return null;

        const keys = hotkey.toString().split('+').map(x => x.trim()).filter(Boolean);
        if (keys.length === 0) return null;

        const modifiers = new Set();
        let key = null;

        keys.forEach(item => {
            const value = this.normalizeKey(item);
            if (!value) return;

            if (value === 'ctrl' || value === 'alt' || value === 'shift' || value === 'meta')
                modifiers.add(value);
            else
                key = value;
        });

        if (!key) return null;

        const result = [];
        if (modifiers.has('ctrl')) result.push('ctrl');
        if (modifiers.has('alt')) result.push('alt');
        if (modifiers.has('shift')) result.push('shift');
        if (modifiers.has('meta')) result.push('meta');
        result.push(key);
        return result.join('+');
    },
    getEventHotkey: function (event) {
        const key = this.normalizeKey(event.key);
        if (!key || key === 'ctrl' || key === 'alt' || key === 'shift' || key === 'meta')
            return null;

        const result = [];
        if (event.ctrlKey) result.push('ctrl');
        if (event.altKey) result.push('alt');
        if (event.shiftKey) result.push('shift');
        if (event.metaKey) result.push('meta');
        result.push(key);
        return result.join('+');
    },
    ensureHandler: function () {
        if (this.hotkeyHandler) return;

        this.hotkeyHandler = event => {
            if (event.repeat) return;

            const hotkey = this.getEventHotkey(event);
            if (!hotkey) return;

            let matched = false;
            for (const group of this.hotkeys.values()) {
                const info = group.get(hotkey);
                if (!info) continue;

                matched = true;
                info.invoker.invokeMethodAsync(info.invoke, hotkey);
            }

            if (matched) {
                event.preventDefault();
                event.stopPropagation();
            }
        };

        document.addEventListener('keydown', this.hotkeyHandler);
    },
    clearHandler: function () {
        if (this.hotkeys.size > 0 || !this.hotkeyHandler) return;

        document.removeEventListener('keydown', this.hotkeyHandler);
        this.hotkeyHandler = null;
    },
    register: function (owner, invoker, hotkeys) {
        if (!owner || !invoker || !hotkeys) return;

        const items = Array.isArray(hotkeys) ? hotkeys : [hotkeys];
        const group = this.hotkeys.get(owner) || new Map();

        items.forEach(item => {
            const hotkey = this.normalizeHotkey(item?.key || item?.Key);
            const invoke = item?.invoke || item?.Invoke;
            if (hotkey && invoke)
                group.set(hotkey, { invoker: invoker, invoke: invoke });
        });

        if (group.size > 0) {
            this.hotkeys.set(owner, group);
            this.ensureHandler();
        }
    },
    dispose: function (owner, hotkeys) {
        if (!owner || !this.hotkeys.has(owner)) return;

        if (!hotkeys || hotkeys.length === 0) {
            this.hotkeys.delete(owner);
            this.clearHandler();
            return;
        }

        const items = Array.isArray(hotkeys) ? hotkeys : [hotkeys];
        const group = this.hotkeys.get(owner);

        items.forEach(item => {
            const hotkey = this.normalizeHotkey(item);
            if (hotkey)
                group.delete(hotkey);
        });

        if (group.size === 0)
            this.hotkeys.delete(owner);

        this.clearHandler();
    }
};

window.KNotify = {
    conn: null,
    handlers: new Map(),
    init: function (invoker, info) {
        const connection = new signalR.HubConnectionBuilder().withUrl(info.notifyUrl).build();
        const forceLogoutHandler = message => {
            //console.log("[KNotify] ForceLogout received:", message);
            invoker.invokeMethodAsync(info.showForceLogout, message);
        };
        const notifyLayoutHandler = message => invoker.invokeMethodAsync(info.showNotify, message);
        connection.on(info.forceLogout, forceLogoutHandler);
        connection.on(info.notifyLayout, notifyLayoutHandler);
        this.handlers.set(info.forceLogout, forceLogoutHandler);
        this.handlers.set(info.notifyLayout, notifyLayoutHandler);

        const registerSession = () => {
            const sessionId = sessionStorage.getItem('sessionId');
            //console.log("[KNotify] RegisterSession try:", sessionId);
            if (sessionId) {
                connection.invoke("RegisterSession", sessionId)
                    .then(() => console.log("[KNotify] RegisterSession success:", sessionId))
                    .catch(err => console.error("[KNotify] RegisterSession failed:", err?.toString?.() ?? err));
            }
        };

        connection.onreconnected(() => {
            //console.log("[KNotify] onreconnected");
            registerSession();
        });

        connection.start().then(function () {
            console.log("SignalR连接已建立");
            registerSession();
        }).catch(function (err) {
            console.error("SignalR连接错误: " + err.toString());
        });
        this.conn = connection;
    },
    addSession: function (sessionId) {
        if (sessionId) {
            sessionStorage.setItem("sessionId", sessionId);
            if (this.conn && this.conn.state === signalR.HubConnectionState.Connected) {
                this.conn.invoke("RegisterSession", sessionId)
                    .then(() => console.log("[KNotify] RegisterSession success(addSession):", sessionId))
                    .catch(err => console.error("[KNotify] RegisterSession failed(addSession):", err?.toString?.() ?? err));
            }
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
