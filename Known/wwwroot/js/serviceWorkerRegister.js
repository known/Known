let deferredPrompt = null
window.addEventListener("beforeinstallprompt", (e) => {
    console.log('beforeinstallprompt-------')
    deferredPrompt = e
    window.addToDesktop = function addToDesktop() {
        deferredPrompt.prompt()
    }
})

window.addEventListener("appinstalled", () => {
    deferredPrompt = null;
});

if ('serviceWorker' in navigator) {
    window.addEventListener('load', function () {
        navigator.serviceWorker.register('/_content/Known/js/serviceWorker.js').then(function (registration) {
            console.log('ServiceWorker registration successful with scope: ', registration.scope);
        }, function (err) {
            console.error('ServiceWorker registration failed: ', err);
        });
    });
}