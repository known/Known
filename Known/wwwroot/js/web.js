window.KUtils = {
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