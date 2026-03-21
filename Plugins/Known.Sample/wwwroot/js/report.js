window.KReport = {
    designers: new Map(),
    initDesigner: function (designerId, invoker) {
        const root = document.getElementById(designerId);
        if (!root || !invoker) return;

        const old = this.designers.get(designerId);
        if (old?.cleanup) old.cleanup();

        let dragId = null;
        const bindItems = () => {
            root.querySelectorAll('[data-report-field]').forEach(item => {
                item.ondragstart = () => {
                    dragId = item.getAttribute('data-report-field');
                    item.classList.add('dragging');
                };
                item.ondragend = () => {
                    item.classList.remove('dragging');
                };
            });
        };

        const selected = root.querySelector('[data-report-list="selected"] .designer-items');
        const source = root.querySelector('[data-report-list="source"] .designer-items');
        if (!selected || !source) return;

        const setDrop = target => {
            target.ondragover = e => {
                e.preventDefault();
                target.classList.add('drag-over');
            };
            target.ondragleave = () => target.classList.remove('drag-over');
            target.ondrop = async e => {
                e.preventDefault();
                target.classList.remove('drag-over');
                if (!dragId) return;

                const ids = Array.from(selected.querySelectorAll('[data-report-field]')).map(i => i.getAttribute('data-report-field'));
                if (!ids.includes(dragId)) {
                    ids.push(dragId);
                }

                const dragging = selected.querySelector('.dragging');
                const after = Array.from(selected.querySelectorAll('[data-report-field]:not(.dragging)')).find(item => {
                    const rect = item.getBoundingClientRect();
                    return e.clientY < rect.top + rect.height / 2;
                });

                if (dragging && dragging.parentElement === selected) {
                    const currentIds = Array.from(selected.querySelectorAll('[data-report-field]')).map(i => i.getAttribute('data-report-field')).filter(Boolean);
                    const newIds = currentIds.filter(id => id !== dragId);
                    const index = after ? newIds.indexOf(after.getAttribute('data-report-field')) : -1;
                    if (index >= 0)
                        newIds.splice(index, 0, dragId);
                    else
                        newIds.push(dragId);
                    await invoker.invokeMethodAsync('UpdateDesignerFields', newIds);
                } else {
                    const currentIds = Array.from(selected.querySelectorAll('[data-report-field]')).map(i => i.getAttribute('data-report-field')).filter(Boolean);
                    if (!currentIds.includes(dragId))
                        currentIds.push(dragId);
                    await invoker.invokeMethodAsync('UpdateDesignerFields', currentIds);
                }
                dragId = null;
            };
        };

        bindItems();
        setDrop(selected);
        setDrop(source);

        this.designers.set(designerId, {
            cleanup: () => {
                root.querySelectorAll('[data-report-field]').forEach(item => {
                    item.ondragstart = null;
                    item.ondragend = null;
                });
                [selected, source].forEach(item => {
                    item.ondragover = null;
                    item.ondragleave = null;
                    item.ondrop = null;
                });
            }
        });
    }
};