(function () {
    const state = {
        device: null,
        server: null,
        service: null,
        writeCharacteristic: null,
        notifyCharacteristic: null,
        dotnet: null,
        handlers: new Map()
    };

    function ensureBluetooth() {
        if (!navigator.bluetooth) {
            throw new Error('当前浏览器不支持 Web Bluetooth。');
        }
    }

    function ensureConnected() {
        if (!state.server || !state.server.connected || !state.service) {
            throw new Error('蓝牙设备尚未连接。');
        }
    }

    async function getCharacteristic(uuid) {
        ensureConnected();
        return await state.service.getCharacteristic(uuid);
    }

    function bytesToHex(bytes) {
        return Array.from(bytes).map(x => x.toString(16).padStart(2, '0')).join('').toUpperCase();
    }

    function removeHandler(uuid) {
        const current = state.handlers.get(uuid);
        if (current && current.characteristic && current.listener) {
            current.characteristic.removeEventListener('characteristicvaluechanged', current.listener);
        }
        state.handlers.delete(uuid);
    }

    async function invokeDotNet(method, arg) {
        if (state.dotnet) {
            if (typeof arg === 'undefined') {
                await state.dotnet.invokeMethodAsync(method);
            } else {
                await state.dotnet.invokeMethodAsync(method, arg);
            }
        }
    }

    window.KSampleBle = {
        async connect(dotnet, serviceUuid, writeCharacteristicUuid, notifyCharacteristicUuid) {
            ensureBluetooth();
            state.dotnet = dotnet;
            state.device = await navigator.bluetooth.requestDevice({
                filters: [{ services: [serviceUuid] }],
                optionalServices: [serviceUuid]
            });
            state.device.addEventListener('gattserverdisconnected', async () => {
                state.server = null;
                state.service = null;
                state.writeCharacteristic = null;
                state.notifyCharacteristic = null;
                await invokeDotNet('OnDisconnect');
            });
            state.server = await state.device.gatt.connect();
            state.service = await state.server.getPrimaryService(serviceUuid);
            state.writeCharacteristic = writeCharacteristicUuid ? await getCharacteristic(writeCharacteristicUuid) : null;
            state.notifyCharacteristic = notifyCharacteristicUuid ? await getCharacteristic(notifyCharacteristicUuid) : null;
            return {
                id: state.device.id,
                name: state.device.name,
                isConnected: state.server.connected
            };
        },
        async disconnect() {
            for (const key of Array.from(state.handlers.keys())) {
                removeHandler(key);
            }
            if (state.device && state.device.gatt && state.device.gatt.connected) {
                state.device.gatt.disconnect();
            }
            state.server = null;
            state.service = null;
            state.writeCharacteristic = null;
            state.notifyCharacteristic = null;
        },
        async startNotifications(uuid) {
            const characteristic = state.notifyCharacteristic && (!uuid || state.notifyCharacteristic.uuid === uuid)
                ? state.notifyCharacteristic
                : await getCharacteristic(uuid);
            await characteristic.startNotifications();
            removeHandler(uuid);
            const listener = async e => {
                const bytes = new Uint8Array(e.target.value.buffer.slice(0));
                const hex = bytesToHex(bytes);
                let text = '';
                try {
                    text = new TextDecoder().decode(bytes);
                } catch {
                    text = '';
                }
                const value = text ? `${hex} | ${text}` : hex;
                await invokeDotNet('OnReceiveData', value);
            };
            characteristic.addEventListener('characteristicvaluechanged', listener);
            state.handlers.set(uuid, { characteristic, listener });
            state.notifyCharacteristic = characteristic;
        },
        async stopNotifications(uuid) {
            const characteristic = state.notifyCharacteristic && (!uuid || state.notifyCharacteristic.uuid === uuid)
                ? state.notifyCharacteristic
                : await getCharacteristic(uuid);
            await characteristic.stopNotifications();
            removeHandler(uuid);
        },
        async writeText(uuid, text) {
            const characteristic = state.writeCharacteristic && (!uuid || state.writeCharacteristic.uuid === uuid)
                ? state.writeCharacteristic
                : await getCharacteristic(uuid);
            const bytes = new TextEncoder().encode(text || '');
            if (characteristic.writeValueWithResponse) {
                await characteristic.writeValueWithResponse(bytes);
            } else {
                await characteristic.writeValue(bytes);
            }
            state.writeCharacteristic = characteristic;
        },
        async writeHex(uuid, hex) {
            const characteristic = state.writeCharacteristic && (!uuid || state.writeCharacteristic.uuid === uuid)
                ? state.writeCharacteristic
                : await getCharacteristic(uuid);
            const value = (hex || '').replace(/\s+/g, '');
            if (!value || value.length % 2 !== 0) {
                throw new Error('HEX命令格式无效。');
            }
            const bytes = new Uint8Array(value.match(/.{1,2}/g).map(x => parseInt(x, 16)));
            if (characteristic.writeValueWithResponse) {
                await characteristic.writeValueWithResponse(bytes);
            } else {
                await characteristic.writeValue(bytes);
            }
            state.writeCharacteristic = characteristic;
        },
        async dispose() {
            await this.disconnect();
            state.dotnet = null;
        }
    };
})();