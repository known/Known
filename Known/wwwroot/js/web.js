function KScanner(invoker, videoId) {
    var deviceId;
    let reader = new ZXing.BrowserMultiFormatReader();
    reader.getVideoInputDevices().then(devices => deviceId = devices[0].deviceId);

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

window.bluetooth = {
    checkSupport: function () {
        return navigator.bluetooth ? true : false;
    },
    getPairedDevices: async function (invoker) {
        try {
            console.log(navigator.bluetooth);
            const devices = await navigator.bluetooth.getAvailability();
            console.log(devices);
            return devices.map(device => ({
                code: device.id,
                name: device.name || '未知设备',
                data: device.uuids
            }));
        } catch (error) {
            console.error('获取已配对设备失败:', error);
            return [];
        }
    },
    startDiscovery: async function (invoker) {
        try {
            const options = {
                acceptAllDevices: true,
                optionalServices: ['generic_access']
            };
            const device = await navigator.bluetooth.requestDevice(options);

            if (device) {
                invoker.invokeMethodAsync('OnDiscovered', {
                    code: device.id,
                    name: device.name || '未知设备',
                    data: device.uuids
                });
            }

            invoker.invokeMethodAsync('OnScanComplete');
        } catch (error) {
            console.error('蓝牙设备发现失败:', error);
            invoker.invokeMethodAsync('OnScanComplete');
        }
    },
    connectToDevice: async function (invoker, deviceId) {
        try {
            const options = {
                acceptAllDevices: true,
                optionalServices: ['generic_access']
            };

            const device = await navigator.bluetooth.requestDevice(options);

            if (device && device.id === deviceId) {
                const server = await device.gatt.connect();

                // 监听断开连接事件
                device.addEventListener('gattserverdisconnected', () => {
                    invoker.invokeMethodAsync('OnDisconnected');
                });

                return true;
            }
            return false;
        } catch (error) {
            console.error('连接蓝牙设备失败:', error);
            return false;
        }
    },
    disconnectDevice: async function () {
        try {
            const devices = await navigator.bluetooth.getDevices();
            for (const device of devices) {
                if (device.gatt.connected) {
                    device.gatt.disconnect();
                }
            }
        } catch (error) {
            console.error('断开蓝牙设备连接失败:', error);
        }
    },
    sendDataToDevice: async function (data) {
        try {
            // 这里需要根据具体的蓝牙设备服务UUID和特征UUID进行实现
            // 以下是示例代码，实际使用时需要修改为您的设备特定的UUID
            const devices = await navigator.bluetooth.getDevices();
            if (devices.length > 0 && devices[0].gatt.connected) {
                const service = await devices[0].gatt.getPrimaryService('generic_access');
                const characteristic = await service.getCharacteristic('gap.device_name');
                // 将字符串转换为ArrayBuffer
                const encoder = new TextEncoder();
                const value = encoder.encode(data);
                await characteristic.writeValue(value);
            }
        } catch (error) {
            console.error('发送数据到蓝牙设备失败:', error);
            throw error;
        }
    }
};

$(function () {
    window.Prism = window.Prism || {};
    Prism.disableWorkerMessageHandler = true;
    Prism.manual = true;
});