var MSG_PROTECT = '请尊重程序猿的劳动成果！';
document.onkeydown = function () {
    var e = window.event || arguments[0];
    if (e.keyCode === 123) {
        alert(MSG_PROTECT);
        return false;
    } else if (e.ctrlKey && e.shiftKey && e.keyCode === 73) {
        alert(MSG_PROTECT);
        return false;
    } else if (e.ctrlKey && e.keyCode === 85) {
        alert(MSG_PROTECT);
        return false;
    } else if (e.ctrlKey && e.keyCode === 83) {
        alert(MSG_PROTECT);
        return false;
    }
};
document.oncontextmenu = function () {
    alert(MSG_PROTECT);
    return false;
};